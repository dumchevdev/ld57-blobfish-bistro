using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime._Game.Scripts.Runtime.CMS;
using Game.Runtime._Game.Scripts.Runtime.CMS.Components.Commons;
using Game.Runtime._Game.Scripts.Runtime.CMS.Components.Gameplay;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers.States;
using Game.Runtime.CMS;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Runtime._Game.Scripts.Runtime.Services.Game
{
    public class GameQueueManager : IDisposable
    {
        private readonly CMSEntity _clientModel;
        private readonly float _spawnInterval;
        private int _cachedFirstClientId;
        
        private readonly List<CustomerInQueueData> _queueClients = new();
        private readonly List<CustomerInQueuePointData> _queuePoints;
        
        private readonly CustomerFactory _customerFactory = new();

        private CancellationTokenSource _spawnTokenSource;
        private CancellationTokenSource _queueTokenSource;
        
        public GameQueueManager()
        {
            var queueEntity = CMSProvider.GetEntity(CMSPrefabs.Gameplay.ClientQueue);
            var queuePrefab = queueEntity.GetComponent<PrefabComponent>().Prefab;
            var queue = Object.Instantiate(queuePrefab);
            queue.name = nameof(GameQueueManager);
            
            _queuePoints = new List<CustomerInQueuePointData>();
            var clientPoints = queue.GetComponentsInChildren<Transform>();
            for (int i = 1; i < clientPoints.Length; i++)
                _queuePoints.Add(new CustomerInQueuePointData(clientPoints[i]));
            
            _clientModel = CMSProvider.GetEntity(CMSPrefabs.Gameplay.Client);
            _spawnInterval = queueEntity.GetComponent<ClientsQueueComponent>().SpawnInterval;

            _cachedFirstClientId = -1;
        }

        public void Start(List<CustomerData> clients)
        {
            StartClientSpawning(clients).Forget();
            StartCheckingQueue().Forget();
        }
        
        public CustomerData DequeueFirstClient()
        {
            if (_queueClients.Count > 0)
            {
                var client = _queueClients[0];
                
                _queuePoints[0].IsOccupied = false;
                _queueClients.RemoveAt(0);
                
                return client.CustomerData;
            }

            return null;
        }

        public bool IsFirstClientInQueue(int clientId)
        {
            return _queueClients.Count > 0 && _queueClients[0].CustomerData.Id == clientId;
        }

        private async UniTask StartClientSpawning(List<CustomerData> clients)
        {
            _spawnTokenSource?.Cancel();
            _spawnTokenSource = new CancellationTokenSource();
            
            while (_spawnTokenSource != null && !_spawnTokenSource.IsCancellationRequested)
            {
                await UniTask.WaitForSeconds(_spawnInterval, cancellationToken: _spawnTokenSource.Token);

                if (_queueClients.Count < _queuePoints.Count)
                {
                    var queuePoint = _queuePoints[_queueClients.Count];
                    if (!queuePoint.IsOccupied)
                    {
                        queuePoint.IsOccupied = true;
                        CreateClient(clients, _queueClients.Count);
                    }
                }
            }
        }
        
        private async UniTask StartCheckingQueue()
        {
            _queueTokenSource?.Cancel();
            _queueTokenSource = new CancellationTokenSource();
            
            while (_queueTokenSource != null && !_queueTokenSource.IsCancellationRequested)
            {
                MoveQueueForward();
                await UniTask.Yield(cancellationToken: _queueTokenSource.Token, cancelImmediately: true);
            }
        }
        
        private void CreateClient(List<CustomerData> clients, int positionIndex)
        {
            var client = _customerFactory.CreateClient(_clientModel);
            clients.Add(client);
            
            var queueClientData = new CustomerInQueueData(client, positionIndex);
            _queueClients.Add(queueClientData);

            if (positionIndex == 0)
                SetupFirstQueueClient(client);

            client.Movable.MoveToPoint(_queuePoints[queueClientData.PointIndex].Point.position).Forget();
        }
        
        public void ReturnClient(CustomerData customer)
        {
            var queueClient = _queueClients.Find(x => x.CustomerData.Id == customer.Id);
            if (queueClient != null)
            {
                _queuePoints[queueClient.PointIndex].IsOccupied = false;
                _queueClients.Remove(queueClient);
            }
            
            //client.Dispose();
            //_clientFactory.Return(client);
        }
        
        private void MoveQueueForward()
        {
            for (int i = 0; i < _queueClients.Count; i++)
            {
                var client = _queueClients[i];
                var targetPoint = _queuePoints[i];
            
                if (!targetPoint.IsOccupied || targetPoint == _queuePoints[client.PointIndex])
                {
                    _queuePoints[client.PointIndex].IsOccupied = false;
                    client.PointIndex = i;
                    targetPoint.IsOccupied = true;

                    client.CustomerData.Movable
                        .MoveToPoint(targetPoint.Point.position, _queueTokenSource.Token).Forget();
                }

                if (client.PointIndex == 0)
                    SetupFirstQueueClient(client.CustomerData);
            }
        }
        
        private void SetupFirstQueueClient(CustomerData customer)
        {
            if (_cachedFirstClientId == customer.Id) return;
            _cachedFirstClientId = customer.Id;
            customer.StateMachine.ChangeState<WaitingInQueueClientState>();
        }

        public void Dispose()
        {
            //_clientFactory.Dispose();
            
            _queueTokenSource?.Dispose();
            _queueTokenSource = null;
            
            _spawnTokenSource?.Dispose();
            _spawnTokenSource = null;
        }
    }
}