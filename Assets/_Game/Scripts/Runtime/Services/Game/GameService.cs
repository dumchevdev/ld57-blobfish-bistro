using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime._Game.Scripts.Runtime.CMS;
using Game.Runtime._Game.Scripts.Runtime.CMS.Components.Commons;
using Game.Runtime._Game.Scripts.Runtime.CMS.Components.Gameplay;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Character;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers.States;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Dishes;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.HUD;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Interactable;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Kitchen;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Level;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Tables;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Tables.Strategies;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Services.Save;
using Game.Runtime._Game.Scripts.Runtime.Services.Statistics;
using Game.Runtime._Game.Scripts.Runtime.Services.UI;
using Game.Runtime._Game.Scripts.Runtime.Utils.Extensions;
using Game.Runtime._Game.Scripts.Runtime.Utils.Сonstants;
using Game.Runtime.CMS;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Game.Runtime._Game.Scripts.Runtime.Services.Game
{
    public class GameService : IService, IInitializable, IDisposable 
    {
        public bool IsSelectedClient => _selectedCustomerData != null;
        
        private CMSEntity _clientModel;
        private readonly float _spawnInterval;
        private CustomerData _selectedCustomerData;
        
        public GameData GameData { get; }
        
        private readonly List<TableData> _tables = new();
        private readonly List<CustomerData> _clients = new();
        private readonly List<OrderData> _orders = new();
        private readonly List<TableData> _cachedFreeTables = new();

        private readonly GameQueueManager _queueManager;
        private readonly CharacterCommandManager _commandManager;
        
        private readonly GameSettingsComponent _gameSettings;
        
        private CancellationTokenSource _gameTokenSource;
        private CancellationTokenSource _selectedClientToken;

        public GameService()
        {
            GameData = new GameData();
            _queueManager = new GameQueueManager();
            _commandManager = new CharacterCommandManager();
            _gameTokenSource = new CancellationTokenSource();
            
            _gameSettings = CMSProvider.GetEntity(CMSPrefabs.Gameplay.GameSettings).GetComponent<GameSettingsComponent>();
            
            InitializeTables();
        }
        
        public void Initialize()
        {
            ServicesProvider.GetService<SessionTimerService>().OnSessionTimerFinished += OnSessionTimerFinished;
        }


        public float GetCurrentGoal()
        {
            return Mathf.Round(_gameSettings.GoldRequired + _gameSettings.GoalMod * Mathf.Log(ServicesProvider.GetService<SaveService>().SaveData.Level, 2));
        }

        public void StartGameLoop()
        {
            _queueManager.Start(_clients);
        }

        public void SelectQueueClient(int clientId)
        {
            if (_selectedCustomerData != null)
            {
                _selectedCustomerData.Behaviour.Settings.OutlineColor = Color.white;
                _selectedCustomerData.Behaviour.Settings.IsClickable = true;
                _selectedCustomerData.Behaviour.Settings.IsHighlightable = true;
                _selectedCustomerData.Behaviour.HideOutline();
                _selectedCustomerData = null;
            }
            
            var clientData = _clients.Find(client => client.Id == clientId);
            if (clientData != null)
            {
                _selectedCustomerData = clientData;
                _selectedCustomerData.Behaviour.Settings.OutlineColor = Color.green;
                _selectedCustomerData.Behaviour.Settings.IsClickable = false;
                _selectedCustomerData.Behaviour.Settings.IsHighlightable = false;
                _selectedCustomerData.Behaviour.ShowOutline();
                
                ShowFreeTables().Forget();
            }
        }

        private async UniTask ShowFreeTables()
        {
            _cachedFreeTables.Clear();
            _selectedClientToken?.Cancel();
            _selectedClientToken = new CancellationTokenSource();
            
            var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(_gameTokenSource.Token, _selectedClientToken.Token);

            try
            {
                while (IsSelectedClient && _selectedClientToken != null && !linkedToken.IsCancellationRequested)
                {
                    foreach (var table in _tables)
                    {
                        if (GetOrderByTable(table.Id) == null)
                        {
                            table.Behaviour.Settings.OutlineColor = Color.green;
                            table.Behaviour.Settings.IsHighlightable = false;
                            table.Behaviour.ShowOutline();
                            _cachedFreeTables.Add(table);
                        }
                    }
                    
                    await UniTask.Yield(cancellationToken: linkedToken.Token, cancelImmediately: true);
                }
            }
            finally
            {
                foreach (var table in _cachedFreeTables)
                {
                    table.Behaviour.Settings.OutlineColor = Color.white;
                    table.Behaviour.Settings.IsHighlightable = true;
                    table.Behaviour.HideOutline();
                }
                
                _selectedClientToken?.Dispose();
                _selectedClientToken = null;
            }
        }

        public void MoveCharacter(Vector3 position)
        {
            _commandManager.AddCommand(() => ServicesProvider.GetService<CharacterService>().Movable.MoveToPoint(position));
        }
        
        public async UniTask PutQueueClientAtTable(int tableId)
        {
            if (IsSelectedClient)
            {
                var tableData = _tables.Find(table => table.Id == tableId);
                if (tableData == null) return;

                var clientData = _queueManager.DequeueCustomer(_selectedCustomerData);
                _selectedCustomerData = null;
                
                clientData.Behaviour.Settings.OutlineColor = Color.white;
                clientData.Behaviour.HideOutline();
                clientData.MoodChecker.ResetMoodTimer();
                
                _orders.Add(new OrderData(clientData, tableData));
                
                await clientData.Movable.MoveToPoint(tableData.Behaviour.ClientPoint.position, _gameTokenSource.Token);
                clientData.Behaviour.SetBlockFlipper(true);
                clientData.Behaviour.ForceFlip(tableData.Behaviour.IsRight);
                
                StartCustomerBrowsingMenu(clientData).Forget();
            }
        }
        
        public OrderData GetOrderByTable(int tableId)
        {
            return _orders.FirstOrDefault(data => data.TableData.Id == tableId);
        }
        
        public OrderData GetOrderByClient(int clientId)
        {
            return _orders.FirstOrDefault(data => data.CustomerData.Id == clientId);
        }
        
        public void TakeOrder(OrderData orderData)
        {
            orderData.OrderAlreadyTaken = true;
            
            var randomFood = CMSProvider.GetEntity(CMSPrefabs.Gameplay.DishesLibrary).GetComponent<DishesLibraryComponent>().Dishes.GetRandom();
            orderData.DinnerId = randomFood.Id;

            MoveCharacter(orderData.TableData.Behaviour.CharacterPoint.position);
            _commandManager.AddCommand(() =>
            {
                if (orderData.IsClosed)
                {
                    return UniTask.CompletedTask;
                }
                ServicesProvider.GetService<KitchenService>().Enqueue(orderData.DinnerId);
                orderData.CustomerData.StateMachine.ChangeState<WaitingFoodClientState>();
                orderData.TableData.Behaviour.InteractionStrategy = new PutFoodTableInteraction();
                return UniTask.CompletedTask;
            });
        }

        public void TakeFood(string foodId, DinnerBehaviour dinnerBehaviour, DinnerPointData dinnerPointData)
        {
            dinnerBehaviour.InteractionStrategy = new EmptyInteraction();
            
            MoveCharacter(dinnerBehaviour.Point.position);
            _commandManager.AddCommand(() =>
            {
                var characterService = ServicesProvider.GetService<CharacterService>();
                var characterHand = characterService.GetFreeHand();
                if (characterHand != null)
                {
                    dinnerBehaviour.gameObject.SetActive(false);
                    characterHand.DinnerData = new DinnerData(foodId, dinnerBehaviour);
                    
                    var foodComponent = CMSProvider.GetEntity(CMSPrefabs.Gameplay.DishesLibrary)
                        .GetComponent<DishesLibraryComponent>().Dishes.First(food => food.Id == foodId);
                    characterService.HandsVisual.SetHandSprite(foodComponent.Sprite, characterHand.IsRightHand);
                    
                    ServicesProvider.GetService<KitchenService>().ReturnFoodPoint(dinnerPointData);
                }
                return UniTask.CompletedTask;
            });
        }
        
        public void ResetCharacterHands(Vector3 trashPosition)
        {
            MoveCharacter(trashPosition);
            _commandManager.AddCommand(() =>
            {
                var characterService = ServicesProvider.GetService<CharacterService>();
                var kitchenService = ServicesProvider.GetService<KitchenService>();

                var leftHand = characterService.CharacterData.LeftHand;
                if (leftHand.DinnerData != null && leftHand.DinnerData.Behaviour != null)
                {
                    kitchenService.ReturnFoodBehaviour(leftHand.DinnerData.Behaviour);
                    leftHand.DinnerData = null;
                }

                var rightHand = characterService.CharacterData.RightHand;
                if (rightHand.DinnerData != null && rightHand.DinnerData.Behaviour != null)
                {
                    kitchenService.ReturnFoodBehaviour(rightHand.DinnerData.Behaviour);
                    rightHand.DinnerData = null;
                }
                
                characterService.HandsVisual.ResetHands();

                return UniTask.CompletedTask;
            });
        }
        
        public void PutOrderFood(OrderData orderData)
        {
            MoveCharacter(orderData.TableData.Behaviour.CharacterPoint.position);
            _commandManager.AddCommand(() =>
            {
                var characterService = ServicesProvider.GetService<CharacterService>();

                if (orderData.IsClosed)
                {
                    if (characterService.TryGetHandWithFood(orderData.DinnerId, out var dataForRemove))
                    {
                        if (orderData.DinnerBehaviour != null)
                            ServicesProvider.GetService<KitchenService>().ReturnFoodBehaviour(orderData.DinnerBehaviour);
                        
                        characterService.HandsVisual.ResetHandSprite(dataForRemove.IsRightHand);
                        dataForRemove.DinnerData = null;
                        return UniTask.CompletedTask;
                    }
                }
                
                if (characterService.TryGetHandWithFood(orderData.DinnerId, out var handData))
                {
                    orderData.DinnerBehaviour = handData.DinnerData.Behaviour;
                    orderData.DinnerBehaviour.Settings.IsClickable = false;
                    orderData.DinnerBehaviour.Settings.IsHighlightable = false;
                    orderData.DinnerBehaviour.transform.position = orderData.TableData.Behaviour.FoodPoint.position;
                    orderData.DinnerBehaviour.ResetBehaviour();
                    orderData.DinnerBehaviour.gameObject.SetActive(true);
                    
                    characterService.HandsVisual.ResetHandSprite(handData.IsRightHand);
                    handData.DinnerData = null;

                    orderData.TableData.Behaviour.InteractionStrategy = new MoveToTableInteraction();
                    StartCustomerEatingFood(orderData.CustomerData).Forget();
                }
                
                return UniTask.CompletedTask;
            });
        }

        public void CleanupTable(OrderData orderData)
        {
            MoveCharacter(orderData.TableData.Behaviour.CharacterPoint.position);
            _commandManager.AddCommand(() =>
            { 
                orderData.TableData.Behaviour.Settings.OutlineColor = Color.white;
                orderData.TableData.Behaviour.Settings.IsHighlightable = true;
                orderData.TableData.Behaviour.HideOutline();
                orderData.TableData.Behaviour.InteractionStrategy = new MoveToTableInteraction();

                if (orderData.DinnerBehaviour != null)
                    ServicesProvider.GetService<KitchenService>().ReturnFoodBehaviour(orderData.DinnerBehaviour);
                
                GameData.Money += orderData.Money;
                
                var saveService = ServicesProvider.GetService<SaveService>();
                saveService.UpdateStatisticsData(orderData.CustomerData.ViewId);
                saveService.SaveData.Statistics.Money += orderData.Money;
                ServicesProvider.GetService<HUDService>().UpdateMoneyPanel(saveService.SaveData.Statistics.Money);
                
                _orders.Remove(orderData);
                return UniTask.CompletedTask;
            });
        }

        public void RemoveOrder(OrderData orderData)
        {
            if (orderData.DinnerBehaviour != null)
            {
                orderData.TableData.Behaviour.Settings.OutlineColor = Color.yellow;
                orderData.TableData.Behaviour.Settings.IsHighlightable = false;
                orderData.TableData.Behaviour.ShowOutline();
                orderData.TableData.Behaviour.InteractionStrategy = new CleanupTableInteraction();
            }
            else
            {
                orderData.TableData.Behaviour.InteractionStrategy = new MoveToTableInteraction();
                _orders.Remove(orderData);
            }
        }

        public async UniTask RemoveClient(CustomerData customerData)
        {
            if (_selectedCustomerData == customerData)
                _selectedCustomerData = null;
            
            _clients.Remove(customerData);
            _queueManager.TryRemoveCustomerInQueue(customerData);

            var leavePosition = ServicesProvider.GetService<LevelPointsService>().LeavePoint.position;
            await customerData.Movable.MoveToPoint(leavePosition, _gameTokenSource.Token);
            
            customerData.Dispose();
            
            if (_gameTokenSource != null && !_gameTokenSource.IsCancellationRequested)
                _queueManager.ReturnPool(customerData);
        }

        private void InitializeTables()
        {
            var tablesPrefab = CMSProvider.GetEntity(CMSPrefabs.Gameplay.Tables).GetComponent<PrefabComponent>().Prefab;
            var tableObject = Object.Instantiate(tablesPrefab);
            tableObject.name = "GameTables";
            
            var tableBehaviours = tableObject.GetComponentsInChildren<TableBehaviour>();
            for (int i = 0; i < tableBehaviours.Length; i++)
            {
                var tableBehaviour = tableBehaviours[i];
                tableBehaviour.Id = i;
                tableBehaviour.InteractionStrategy = new MoveToTableInteraction();

                var interactionObject = (InteractableObject)tableBehaviour;
                interactionObject.Settings.IsClickable = true;
                interactionObject.Settings.IsHighlightable = true;
                
                _tables.Add(new TableData(i, tableBehaviours[i]));
            }
        }

        private async UniTask StartCustomerBrowsingMenu(CustomerData customerData)
        {
            if (_gameTokenSource == null || _gameTokenSource.IsCancellationRequested)
                return;
            
            customerData.StateMachine.ChangeState<BrowsingMenuClientState>();
            await UniTask.WaitForSeconds(_gameSettings.CustomerBrowsingMenuTime, cancellationToken: _gameTokenSource.Token);
            customerData.StateMachine.ChangeState<WaitingOrderClientState>();

            var orderData = GetOrderByClient(customerData.Id);
            orderData.TableData.Behaviour.InteractionStrategy = new TakeOrderTableInteraction();
        }

        private async UniTask StartCustomerEatingFood(CustomerData customerData)
        {
            customerData.StateMachine.ChangeState<EatingOrderFoodClientState>();
            await UniTask.WaitForSeconds(_gameSettings.CustomerEatingTime, cancellationToken: _gameTokenSource.Token);
            
            var orderData = GetOrderByClient(customerData.Id);
            var emptyPlate = CMSProvider.GetEntity(CMSPrefabs.Gameplay.EmptyPlate).GetComponent<SpriteComponent>().Sprite;
            orderData.DinnerBehaviour.SetFoodSprite(emptyPlate);

            var dishesLibrary = CMSProvider.GetEntity(CMSPrefabs.Gameplay.DishesLibrary).GetComponent<DishesLibraryComponent>();
            var dinnerComponent = dishesLibrary.Dishes.First(dinner => dinner.Id == orderData.DinnerId);
            orderData.Money += Mathf.Round(dinnerComponent.BasePrice + _gameSettings.OrderMod * Mathf.Log(ServicesProvider.GetService<SaveService>().SaveData.Level));

            customerData.StateMachine.ChangeState<LeavingClientState>();
        }
        
        private void OnSessionTimerFinished()
        {
            FinishSession().Forget();
            ServicesProvider.GetService<SessionTimerService>().OnSessionTimerFinished -= OnSessionTimerFinished;
        }

        private async UniTask FinishSession()
        {
            await UniTask.WaitUntil(() => _clients.Count == 0 && _orders.Count == 0, cancellationToken: _gameTokenSource.Token);
            await ServicesProvider.GetService<UIService>().FadeIn();
            ServicesProvider.GetService<StatisticsService>().StatisticsData = new StatisticsData
            {
                Goal = GetCurrentGoal(),
                CollectedGolds = GameData.Money
            };
            ServicesProvider.GetService<SaveService>().SaveData.Level++;
            ServicesProvider.GetService<SaveService>().Save();
            SceneManager.LoadScene(Const.ScenesConst.StatisticsScene);
        }
        
        public void Dispose()
        {
            _cachedFreeTables.Clear();
            _queueManager?.Dispose();
            _commandManager?.Dispose();
            _selectedClientToken?.Dispose();
            _selectedClientToken = null;
            _gameTokenSource?.Dispose();
            _gameTokenSource = null;
        }
    }
}