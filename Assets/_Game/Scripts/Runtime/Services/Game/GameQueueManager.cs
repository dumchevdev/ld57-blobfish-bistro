using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime._Game.Scripts.Runtime.CMS;
using Game.Runtime._Game.Scripts.Runtime.CMS.Components.Commons;
using Game.Runtime._Game.Scripts.Runtime.CMS.Components.Gameplay;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers.States;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Utils.Structs;
using Game.Runtime.CMS;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Runtime._Game.Scripts.Runtime.Services.Game
{
    public class GameQueueManager : IDisposable
    {
        private readonly CMSEntity _customerModel;
        private readonly RandomRangeFloat _spawnInterval;
        
        private readonly List<CustomerInQueueData> _queueCustomers = new();
        private readonly List<CustomerInQueuePointData> _queuePoints;
        
        private readonly CustomerFactory _customerFactory = new();

        private CancellationTokenSource _spawnTokenSource;
        private CancellationTokenSource _queueTokenSource;
        
        public GameQueueManager()
        {
            var queuePrefab = CMSProvider.GetEntity(CMSPrefabs.Gameplay.CustomersQueue).GetComponent<PrefabComponent>().Prefab;
            var queue = Object.Instantiate(queuePrefab);
            queue.name = nameof(GameQueueManager);
            
            _queuePoints = new List<CustomerInQueuePointData>();
            var customerPoints = queue.GetComponentsInChildren<Transform>();
            for (int i = 1; i < customerPoints.Length; i++)
                _queuePoints.Add(new CustomerInQueuePointData(customerPoints[i]));
            
            _customerModel = CMSProvider.GetEntity(CMSPrefabs.Gameplay.Customer);
            _spawnInterval = CMSProvider.GetEntity(CMSPrefabs.Gameplay.GameSettings).GetComponent<GameSettingsComponent>().CustomersSpawnInterval;
        }

        public void Start(List<CustomerData> customers)
        {
            StartCustomersSpawning(customers).Forget();
            StartCheckingQueue().Forget();
        }
        
        public CustomerData DequeueCustomer(CustomerData customerData)
        {
            if (_queueCustomers.Count > 0)
            {
                var customer = _queueCustomers.Find(data => data.CustomerData == customerData);
                
                _queuePoints[customer.PointIndex].IsOccupied = false;
                _queueCustomers.RemoveAll(data => data.CustomerData == customerData);
                
                return customer.CustomerData;
            }

            return null;
        }

        private async UniTask StartCustomersSpawning(List<CustomerData> customers)
        {
            _spawnTokenSource?.Cancel();
            _spawnTokenSource = new CancellationTokenSource();
            
            while (_spawnTokenSource != null && !_spawnTokenSource.IsCancellationRequested)
            {
                if (ServicesProvider.GetService<SessionTimerService>().SessionFinished)
                {
                    _spawnTokenSource.Cancel();
                    return;
                }
                
                if (_queueCustomers.Count < _queuePoints.Count)
                {
                    var queuePoint = _queuePoints[_queueCustomers.Count];
                    if (!queuePoint.IsOccupied)
                    {
                        queuePoint.IsOccupied = true;
                        CreateCustomer(customers, _queueCustomers.Count);
                    }
                }
                
                await UniTask.WaitForSeconds(_spawnInterval.Random(), cancellationToken: _spawnTokenSource.Token);
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
        
        private void CreateCustomer(List<CustomerData> customers, int positionIndex)
        {
            var customer = _customerFactory.CreateClient(_customerModel);
            customers.Add(customer);
            
            var queueClientData = new CustomerInQueueData(customer, positionIndex);
            _queueCustomers.Add(queueClientData);

            customer.StateMachine.ChangeState<WaitingInQueueClientState>();
            customer.Movable.MoveToPoint(_queuePoints[queueClientData.PointIndex].Point.position).Forget();
        }
        
        public void TryRemoveCustomerInQueue(CustomerData customer)
        {
            var queueClient = _queueCustomers.Find(x => x.CustomerData.Id == customer.Id);
            if (queueClient != null)
            {
                _queuePoints[queueClient.PointIndex].IsOccupied = false;
                _queueCustomers.Remove(queueClient);
            }
        }

        public void ReturnPool(CustomerData customer)
        {
            _customerFactory.Return(customer);
        }
        
        private void MoveQueueForward()
        {
            for (int i = 0; i < _queueCustomers.Count; i++)
            {
                var customer = _queueCustomers[i];
                var targetPoint = _queuePoints[i];
            
                if (!targetPoint.IsOccupied || targetPoint == _queuePoints[customer.PointIndex])
                {
                    _queuePoints[customer.PointIndex].IsOccupied = false;
                    customer.PointIndex = i;
                    targetPoint.IsOccupied = true;

                    customer.CustomerData.Movable
                        .MoveToPoint(targetPoint.Point.position, _queueTokenSource.Token).Forget();
                }

                customer.CustomerData.StateMachine.ChangeState<WaitingInQueueClientState>();
            }
        }
        
        public void Dispose()
        {
            _customerFactory.Dispose();
            
            _queueTokenSource?.Dispose();
            _queueTokenSource = null;
            
            _spawnTokenSource?.Dispose();
            _spawnTokenSource = null;
        }
    }
}