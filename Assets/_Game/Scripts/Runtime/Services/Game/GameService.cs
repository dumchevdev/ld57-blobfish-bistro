using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Commons;
using Game.Runtime.CMS.Components.Gameplay;
using Game.Runtime.Gameplay.Character;
using Game.Runtime.Gameplay.FoodDelivery;
using Game.Runtime.Gameplay.Interactives;
using Game.Runtime.Gameplay.Level;
using Game.Runtime.ServiceLocator;
using Game.Runtime.Utils.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Runtime.Framework.Services.Game
{
    public class GameService : IService, IDisposable
    {
        public bool IsSelectedClient { get; private set; }

        private GameData _gameData;
        
        private CMSEntity _clientModel;
        private readonly float _spawnInterval;
        
        private readonly List<TableData> _tables = new();
        private readonly List<ClientData> _clients = new();
        private readonly List<OrderData> _orders = new();

        private readonly GameQueueManager _queueManager;
        private readonly UnitCommandManager _commandManager;
        
        private CancellationTokenSource _gameTokenSource;
        private CancellationTokenSource _selectedClientToken;

        public GameService()
        {
            _gameData = new GameData();
            _queueManager = new GameQueueManager();
            _commandManager = new UnitCommandManager();
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
            return _orders.FirstOrDefault(data => data.ClientData.Id == clientId);
        }
        
        public void TakeOrder(OrderData orderData)
        {
            var randomFood = CMSProvider.GetEntity(CMSPrefabs.Gameplay.Foods).GetComponent<FoodsComponent>().Foods.GetRandom();
            orderData.FoodId = randomFood.Id;

            MoveCharacter(orderData.TableData.Behaviour.CharacterPoint.position);
            _commandManager.AddCommand(() =>
            {
                if (orderData.IsClosed)
                {
                    return UniTask.CompletedTask;
                }
                ServiceLocator<FoodDeliveryService>.GetService().Enqueue(orderData.FoodId);
                orderData.ClientData.StateMachine.ChangeState<WaitingFoodClientState>();
                orderData.TableData.Behaviour.InteractionStrategy = new PutFoodTableInteraction();
                return UniTask.CompletedTask;
            });
        }

        public void TakeFood(string foodId, FoodBehaviour foodBehaviour, FoodPointData foodPointData)
        {
            MoveCharacter(foodBehaviour.Point.position);
            _commandManager.AddCommand(() =>
            { 
                var characterHand = ServiceLocator<CharacterService>.GetService().GetFreeHand();
                if (characterHand != null)
                {
                    foodBehaviour.gameObject.SetActive(false);
                    characterHand.FoodData = new FoodData(foodId, foodBehaviour);
                    ServiceLocator<FoodDeliveryService>.GetService().ReturnFoodPoint(foodPointData);
                }
                return UniTask.CompletedTask;
            });
        }

        public void PutOrderFood(OrderData orderData)
        {
            MoveCharacter(orderData.TableData.Behaviour.CharacterPoint.position);
            _commandManager.AddCommand(() =>
            {
                if (orderData.IsClosed)
                {
                    Debug.Log("Прикольчик");
                    if (ServiceLocator<CharacterService>.GetService()
                        .TryGetHandWithFood(orderData.FoodId, out var dataForRemove))
                    {
                        dataForRemove.FoodData = null;
                        return UniTask.CompletedTask;
                    }
                }
                if (ServiceLocator<CharacterService>.GetService().TryGetHandWithFood(orderData.FoodId, out var handData))
                {
                    orderData.FoodBehaviour = handData.FoodData.Behaviour;
                    orderData.FoodBehaviour.Settings.IsClickable = false;
                    orderData.FoodBehaviour.Settings.IsHighlightable = false;
                    orderData.FoodBehaviour.transform.position = orderData.TableData.Behaviour.FoodPoint.position;
                    orderData.FoodBehaviour.gameObject.SetActive(true);
                    handData.FoodData = null;

                    orderData.TableData.Behaviour.InteractionStrategy = new MoveToTableInteraction();
                    StartClientEatingFood(orderData.ClientData).Forget();
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

                if (orderData.FoodBehaviour != null)
                    ServiceLocator<FoodDeliveryService>.GetService().ReturnFoodToPool(orderData.FoodBehaviour);
            
                _orders.Remove(orderData);
                return UniTask.CompletedTask;
            });
        }

        public void RemoveOrder(OrderData orderData)
        {
            if (orderData.FoodBehaviour != null)
            {
                orderData.TableData.Behaviour.InteractionStrategy = new CleanupTableInteraction();
            }
            else
            {
                orderData.TableData.Behaviour.InteractionStrategy = new MoveToTableInteraction();
                _orders.Remove(orderData);
            }
        }

        public async UniTask RemoveClient(ClientData clientData)
        {
            var leavePosition = ServiceLocator<LevelPointsService>.GetService().LeavePoint.position;
            await clientData.Movable.MoveToPoint(leavePosition, _gameTokenSource.Token);
            
            _clients.Remove(clientData);
            _queueManager.ReturnClient(clientData);
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

        private async UniTask StartClientBrowsingMenu(ClientData clientData)
        {
            clientData.StateMachine.ChangeState<BrowsingMenuClientState>();
            await UniTask.WaitForSeconds(3, cancellationToken: _gameTokenSource.Token);
            clientData.StateMachine.ChangeState<WaitingOrderClientState>();

            var orderData = GetOrderByClient(clientData.Id);
            orderData.TableData.Behaviour.InteractionStrategy = new TakeOrderTableInteraction();
        }

        private async UniTask StartClientEatingFood(ClientData clientData)
        {
            clientData.StateMachine.ChangeState<EatingOrderFoodClientState>();
            await UniTask.WaitForSeconds(3, cancellationToken: _gameTokenSource.Token);
            clientData.StateMachine.ChangeState<LeavingClientState>();
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