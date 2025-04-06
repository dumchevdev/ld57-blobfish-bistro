using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Commons;
using Game.Runtime.CMS.Components.Gameplay;
using Game.Runtime.ServiceLocator;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Runtime.Gameplay.Interactives
{
    public class ClientsService : IService, IDisposable
    {
        private readonly List<ClientData> _clients = new();
        private readonly List<QueueClientData> _clientsQueue = new();
        
        private readonly ClientFactory _clientFactory = new();
        private readonly ClientQueueChecker _queueChecker;
        
        private readonly List<QueueClientPointData> _clientQueuePoints = new();
        private readonly CMSEntity _clientModel;
        private readonly float _spawnInterval;
        
        private CancellationTokenSource _spawnTokenSource;
        private CancellationTokenSource _queueTokenSource;
        
        private ClientData _cachedFirstClient;

        private bool _queueMoving;

        public ClientsService()
        {
            var queueEntity = CMSProvider.GetEntity(CMSPrefabs.Gameplay.ClientQueue);
            var queuePrefab = queueEntity.GetComponent<PrefabComponent>().Prefab;
            var queue = Object.Instantiate(queuePrefab);
            queue.name = nameof(ClientsService);

            var clientPoints = queue.GetComponentsInChildren<Transform>();
            for (int i = 1; i < clientPoints.Length; i++)
                _clientQueuePoints.Add(new QueueClientPointData(clientPoints[i]));
            
            _clientModel = CMSProvider.GetEntity(CMSPrefabs.Gameplay.Client);
            _spawnInterval = queueEntity.GetComponent<ClientsQueueComponent>().SpawnInterval;

            _queueChecker = new ClientQueueChecker(this);
        }
        
        public async UniTask StartQueueSpawning()
        {
            _spawnTokenSource?.Cancel();
            _spawnTokenSource = new CancellationTokenSource();
            
            while (_spawnTokenSource != null && !_spawnTokenSource.IsCancellationRequested)
            {
                await UniTask.WaitForSeconds(_spawnInterval, cancellationToken: _spawnTokenSource.Token);

                if (_clientsQueue.Count < _clientQueuePoints.Count)
                {
                    Debug.Log("Try spawn");
                    var queuePoint = _clientQueuePoints[_clientsQueue.Count];
                    if (!queuePoint.IsOccupied)
                    {
                        queuePoint.IsOccupied = true;
                        CreateClient(_clientsQueue.Count);
                    }
                }
            }
        }

        public ClientData GetClient(int id)
        {
            return _clients.Find(client => client.Id == id);
        }

        public ClientData DequeueFirstClient()
        {
            if (_clientsQueue.Count > 0)
            {
                var client = _clientsQueue[0];
                _clientQueuePoints[0].IsOccupied = false;
                _clientsQueue.RemoveAt(0);
                return client.ClientData;
            }

            return null;
        }

        public bool IsFirstQueueClientSelected()
        {
            return _clientsQueue.Count > 0 && _clientsQueue[0].ClientData.State == ClientState.ChoosingTable;
        }

        public void RemoveClient(int id)
        {
            var client = _clients.Find(x => x.Id == id);
            client.Dispose();

            _clients.Remove(client);

            var containsClient = _clientsQueue.Find(x => x.ClientData.Id == id);
            if (containsClient != null)
            {
                _clientQueuePoints[containsClient.PointIndex].IsOccupied = false;
                _clientsQueue.Remove(containsClient);
            }
            
            _clientFactory.Return(client);
        }

        private void CreateClient(int positionIndex)
        {
            var client = _clientFactory.CreateClient(_clientModel);
            _clients.Add(client);
            
            var queueClientData = new QueueClientData(client, positionIndex);
            _clientsQueue.Add(queueClientData);

            if (positionIndex == 0)
                SetupFirstQueueClient(client);
            else
                client.View.SetStrategy(new NotingClientInteraction());

            client.Movable.MoveToPoint(_clientQueuePoints[queueClientData.PointIndex].Point.position).Forget();
        }

        private void SetupFirstQueueClient(ClientData client)
        {
            if (_cachedFirstClient != null && 
                _cachedFirstClient.Id == client.Id) return;

            _cachedFirstClient = client;
            
            client.State = ClientState.WaitingInQueue;
            client.View.SetStrategy(new SelectedClientInteraction());
        }

        public async UniTask MoveQueueForward()
        {
            if (_queueMoving) return;
            _queueMoving = true;
    
            var moveTasks = new List<UniTask>();
    
            _queueTokenSource?.Cancel();
            _queueTokenSource = new CancellationTokenSource();

            try
            {
                for (int i = 0; i < _clientsQueue.Count; i++)
                {
                    var client = _clientsQueue[i];
                    var targetIndex = i; // Каждый клиент перемещается на позицию, равную его месту в очереди
                    var targetPoint = _clientQueuePoints[targetIndex];
            
                    if (!targetPoint.IsOccupied || targetPoint == _clientQueuePoints[client.PointIndex])
                    {
                        moveTasks.Add(client.ClientData.Movable.MoveToPoint(targetPoint.Point.position, 
                            () =>
                            {
                                _clientQueuePoints[client.PointIndex].IsOccupied = false;
                                client.PointIndex = targetIndex;
                                targetPoint.IsOccupied = true;
                                if (client.PointIndex == 0)
                                    SetupFirstQueueClient(client.ClientData);
                            }, externalToken: _queueTokenSource.Token));
                    }
                }

                await UniTask.WhenAll(moveTasks)
                    .AttachExternalCancellation(_queueTokenSource.Token).SuppressCancellationThrow();
            }
            finally
            {
                _queueMoving = false;
                ResetQueueToken();
            }
        }
        
        private void ResetSpawnToken()
        {
            _spawnTokenSource?.Dispose();
            _spawnTokenSource = null;
        }

        private void ResetQueueToken()
        {
            _queueTokenSource?.Dispose();
            _queueTokenSource = null;
        }

        public void Dispose()
        {
            _queueChecker.Dispose();
            
            ResetSpawnToken();
            ResetQueueToken();
            
            foreach (var client in _clients)
                client.Dispose();
            
            _clients.Clear();
            _clientsQueue.Clear();
        }
    }
}