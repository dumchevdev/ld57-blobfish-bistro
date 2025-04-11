using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Commons;
using Game.Runtime.CMS.Components.Gameplay;
using Game.Runtime.Gameplay.Interactives;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Runtime.Framework.Services.Game
{
    public class GameQueueManager : IDisposable
    {
        private readonly CMSEntity _clientModel;
        private readonly float _spawnInterval;
        private int _cachedFirstClientId;
        
        private readonly List<QueueClientData> _queueClients = new();
        private readonly List<QueueClientPointData> _queuePoints;
        
        private readonly ClientFactory _clientFactory = new();

        private CancellationTokenSource _spawnTokenSource;
        private CancellationTokenSource _queueTokenSource;
        
        public GameQueueManager()
        {
            var queueEntity = CMSProvider.GetEntity(CMSPrefabs.Gameplay.ClientQueue);
            var queuePrefab = queueEntity.GetComponent<PrefabComponent>().Prefab;
            var queue = Object.Instantiate(queuePrefab);
            queue.name = nameof(GameQueueManager);
            
            _queuePoints = new List<QueueClientPointData>();
            var clientPoints = queue.GetComponentsInChildren<Transform>();
            for (int i = 1; i < clientPoints.Length; i++)
                _queuePoints.Add(new QueueClientPointData(clientPoints[i]));
            
            _clientModel = CMSProvider.GetEntity(CMSPrefabs.Gameplay.Client);
            _spawnInterval = queueEntity.GetComponent<ClientsQueueComponent>().SpawnInterval;

            _cachedFirstClientId = -1;
        }

        public void Start(List<ClientData> clients)
        {
            StartClientSpawning(clients).Forget();
            StartCheckingQueue().Forget();
        }
        
        public ClientData DequeueFirstClient()
        {
            if (_queueClients.Count > 0)
            {
                var client = _queueClients[0];
                
                _queuePoints[0].IsOccupied = false;
                _queueClients.RemoveAt(0);
                
                return client.ClientData;
            }

            return null;
        }

        public bool IsFirstClientInQueue(int clientId)
        {
            return _queueClients.Count > 0 && _queueClients[0].ClientData.Id == clientId;
        }

        private async UniTask StartClientSpawning(List<ClientData> clients)
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
        
        private void CreateClient(List<ClientData> clients, int positionIndex)
        {
            var client = _clientFactory.CreateClient(_clientModel);
            clients.Add(client);
            
            var queueClientData = new QueueClientData(client, positionIndex);
            _queueClients.Add(queueClientData);

            if (positionIndex == 0)
                SetupFirstQueueClient(client);

            client.Movable.MoveToPoint(_queuePoints[queueClientData.PointIndex].Point.position).Forget();
        }
        
        public void ReturnClient(ClientData client)
        {
            var queueClient = _queueClients.Find(x => x.ClientData.Id == client.Id);
            if (queueClient != null)
            {
                _queuePoints[queueClient.PointIndex].IsOccupied = false;
                _queueClients.Remove(queueClient);
            }
            
            client.Dispose();
            _clientFactory.Return(client);
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

                    client.ClientData.Movable
                        .MoveToPoint(targetPoint.Point.position, _queueTokenSource.Token).Forget();
                }

                if (client.PointIndex == 0)
                    SetupFirstQueueClient(client.ClientData);
            }
        }
        
        private void SetupFirstQueueClient(ClientData client)
        {
            if (_cachedFirstClientId == client.Id) return;
            _cachedFirstClientId = client.Id;
            client.StateMachine.ChangeState<WaitingInQueueClientState>();
        }

        public void Dispose()
        {
            _clientFactory.Dispose();
            
            _queueTokenSource?.Dispose();
            _queueTokenSource = null;
            
            _spawnTokenSource?.Dispose();
            _spawnTokenSource = null;
        }
    }
}