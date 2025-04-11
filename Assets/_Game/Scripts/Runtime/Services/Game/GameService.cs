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
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Interactable;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Kitchen;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Level;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Tables;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Tables.Strategies;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Utils.Extensions;
using Game.Runtime.CMS;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Runtime._Game.Scripts.Runtime.Services.Game
{
    public class GameService : IService, IDisposable
    {
        public bool IsSelectedClient { get; private set; }

        private GameData _gameData;
        
        private CMSEntity _clientModel;
        private readonly float _spawnInterval;
        
        private readonly List<TableData> _tables = new();
        private readonly List<CustomerData> _clients = new();
        private readonly List<OrderData> _orders = new();

        private readonly GameQueueManager _queueManager;
        private readonly PlayerCommandManager _commandManager;
        
        private CancellationTokenSource _gameTokenSource;
        private CancellationTokenSource _selectedClientToken;

        public GameService()
        {
            _gameData = new GameData();
            _queueManager = new GameQueueManager();
            _commandManager = new PlayerCommandManager();
            _gameTokenSource = new CancellationTokenSource();
            
            InitializeTables();
        }

        public void StartGameLoop()
        {
            _queueManager.Start(_clients);
        }

        public void SelectQueueClient(int clientId)
        {
            if (IsSelectedClient) return;
            if (!_queueManager.IsFirstClientInQueue(clientId)) return;
            
            var clientData = _clients.Find(client => client.Id == clientId);
            IsSelectedClient = clientData != null;

            if (clientData != null)
            {
                clientData.View.Settings.OutlineColor = Color.green;
                clientData.View.Settings.IsClickable = false;
                clientData.View.Settings.IsHighlightable = false;
                
                clientData.View.ShowOutline();
                
                ShowFreeTables().Forget();
            }
        }

        private async UniTask ShowFreeTables()
        {
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
                        }
                    }
                    
                    await UniTask.Yield(cancellationToken: linkedToken.Token, cancelImmediately: true);
                }
            }
            finally
            {
                foreach (var table in _tables)
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
            _commandManager.AddCommand(() => ServiceLocator<CharacterService>.GetService().Movable.MoveToPoint(position));
        }
        
        public async UniTask PutQueueClientAtTable(int tableId)
        {
            if (IsSelectedClient)
            {
                var tableData = _tables.Find(table => table.Id == tableId);
                if (tableData == null) return;

                var clientData = _queueManager.DequeueFirstClient();
                IsSelectedClient = false;
                
                clientData.View.Settings.OutlineColor = Color.white;
                clientData.View.HideOutline();
                clientData.MoodChecker.ResetMoodTimer();
                
                _orders.Add(new OrderData(clientData, tableData));
                
                await clientData.Movable.MoveToPoint(tableData.Behaviour.ClientPoint.position, _gameTokenSource.Token);
                
                StartClientBrowsingMenu(clientData).Forget();
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
            
            var randomFood = CMSProvider.GetEntity(CMSPrefabs.Gameplay.Foods).GetComponent<FoodsComponent>().Foods.GetRandom();
            orderData.FoodId = randomFood.Id;

            MoveCharacter(orderData.TableData.Behaviour.CharacterPoint.position);
            _commandManager.AddCommand(() =>
            {
                if (orderData.IsClosed)
                {
                    return UniTask.CompletedTask;
                }
                ServiceLocator<KitchenService>.GetService().Enqueue(orderData.FoodId);
                orderData.CustomerData.StateMachine.ChangeState<WaitingFoodClientState>();
                orderData.TableData.Behaviour.InteractionStrategy = new PutFoodTableInteraction();
                return UniTask.CompletedTask;
            });
        }

        public void TakeFood(string foodId, DinnerBehaviour dinnerBehaviour, FoodPointData foodPointData)
        {
            dinnerBehaviour.InteractionStrategy = new EmptyInteraction();
            
            MoveCharacter(dinnerBehaviour.Point.position);
            _commandManager.AddCommand(() =>
            {
                var characterService = ServiceLocator<CharacterService>.GetService();
                var characterHand = characterService.GetFreeHand();
                if (characterHand != null)
                {
                    dinnerBehaviour.gameObject.SetActive(false);
                    characterHand.DinnerData = new DinnerData(foodId, dinnerBehaviour);
                    
                    var foodComponent = CMSProvider.GetEntity(CMSPrefabs.Gameplay.Foods)
                        .GetComponent<FoodsComponent>().Foods.First(food => food.Id == foodId);
                    characterService.HandsVisual.SetHandSprite(foodComponent.Sprite, characterHand.IsRightHand);
                    
                    ServiceLocator<KitchenService>.GetService().ReturnFoodPoint(foodPointData);
                }
                return UniTask.CompletedTask;
            });
        }

        public void PutOrderFood(OrderData orderData)
        {
            MoveCharacter(orderData.TableData.Behaviour.CharacterPoint.position);
            _commandManager.AddCommand(() =>
            {
                var characterService = ServiceLocator<CharacterService>.GetService();

                if (orderData.IsClosed)
                {
                    if (characterService.TryGetHandWithFood(orderData.FoodId, out var dataForRemove))
                    {

                        characterService.HandsVisual.ResetHandSprite(dataForRemove.IsRightHand);
                        dataForRemove.DinnerData = null;
                        return UniTask.CompletedTask;
                    }
                }
                
                if (characterService.TryGetHandWithFood(orderData.FoodId, out var handData))
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
                    StartClientEatingFood(orderData.CustomerData).Forget();
                }
                
                return UniTask.CompletedTask;
            });
        }

        public void CleanupTable(OrderData orderData)
        {
            MoveCharacter(orderData.TableData.Behaviour.CharacterPoint.position);
            _commandManager.AddCommand(() =>
            { 
                orderData.TableData.Behaviour.InteractionStrategy = new MoveToTableInteraction();

                if (orderData.DinnerBehaviour != null)
                    ServiceLocator<KitchenService>.GetService().ReturnFoodBehaviour(orderData.DinnerBehaviour);
            
                _orders.Remove(orderData);
                return UniTask.CompletedTask;
            });
        }

        public void RemoveOrder(OrderData orderData)
        {
            if (orderData.DinnerBehaviour != null)
            {
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
            _clients.Remove(customerData);
            _queueManager.ReturnClient(customerData);

            var leavePosition = ServiceLocator<LevelPointsService>.GetService().LeavePoint.position;
            await customerData.Movable.MoveToPoint(leavePosition, _gameTokenSource.Token);
            
            customerData.Dispose();
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

        private async UniTask StartClientBrowsingMenu(CustomerData customerData)
        {
            customerData.StateMachine.ChangeState<BrowsingMenuClientState>();
            await UniTask.WaitForSeconds(3, cancellationToken: _gameTokenSource.Token);
            customerData.StateMachine.ChangeState<WaitingOrderClientState>();

            var orderData = GetOrderByClient(customerData.Id);
            orderData.TableData.Behaviour.InteractionStrategy = new TakeOrderTableInteraction();
        }

        private async UniTask StartClientEatingFood(CustomerData customerData)
        {
            customerData.StateMachine.ChangeState<EatingOrderFoodClientState>();
            await UniTask.WaitForSeconds(3, cancellationToken: _gameTokenSource.Token);
            customerData.StateMachine.ChangeState<LeavingClientState>();
        }
        
        public void Dispose()
        {
            _queueManager?.Dispose();
            _commandManager?.Dispose();
            _selectedClientToken?.Dispose();
            _selectedClientToken = null;
            _gameTokenSource?.Dispose();
            _gameTokenSource = null;
        }
    }
}