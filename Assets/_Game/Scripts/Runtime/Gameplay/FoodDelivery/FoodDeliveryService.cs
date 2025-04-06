using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Commons;
using Game.Runtime.Gameplay.Interactives;
using Game.Runtime.ServiceLocator;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Runtime.Gameplay.FoodDelivery
{
    public class FoodDeliveryService : IService, IDisposable
    {
        private readonly List<FoodPointData> _foodPoints;
        private readonly Queue<ClientOrderData> _orderQueue;
        private readonly FoodFactory _foodFactory;

        private CancellationTokenSource _foodTokenSource;
        
        public FoodDeliveryService()
        {
            _foodPoints = new List<FoodPointData>();
            _orderQueue = new Queue<ClientOrderData>();
            
            var deliveryPrefab = CMSProvider.GetEntity(CMSPrefabs.Gameplay.FoodDeliveryArea).GetComponent<PrefabComponent>().Prefab;
            var deliveryObject = Object.Instantiate(deliveryPrefab);
            deliveryObject.name = nameof(FoodDeliveryService);
            
            _foodFactory = new FoodFactory();

            var foodPoints = deliveryPrefab.GetComponentsInChildren<Transform>();
            for (int i = 1; i < foodPoints.Length; i++)
                _foodPoints.Add(new FoodPointData(foodPoints[i]));

            ApplyFoodOrders().Forget();
        }

        public UniTask Enqueue(ClientOrderData foodOrderData)
        {
            _orderQueue.Enqueue(foodOrderData);
            return UniTask.CompletedTask;
        }

        private bool AnyFoodPointFree()
        {
            return _foodPoints.Any(data => !data.IsOccupied);
        }

        private FoodPointData GetFoodPoint()
        {
            foreach (var dataPoint in _foodPoints)
            {
                if (!dataPoint.IsOccupied)
                {
                    dataPoint.IsOccupied = true;
                    return dataPoint;
                }
            }

            return null;
        }

        public void ReturnFoodToPool(FoodBehaviour foodBehaviour)
        {
            _foodFactory.ReturnFoodBehaviour(foodBehaviour);
        }

        public void ReturnFoodPoint(FoodPointData foodPointData)
        {
            foodPointData.IsOccupied = false;
        }

        private async UniTask ApplyFoodOrders()
        {
            _foodTokenSource = new CancellationTokenSource();
            
            while (_foodTokenSource != null && !_foodTokenSource.IsCancellationRequested)
            {
                if (_orderQueue.Count > 0 && _foodFactory.IsFree && AnyFoodPointFree())
                {
                    var orderData = _orderQueue.Dequeue();
                    var foodPoint = GetFoodPoint();
                    
                    if (foodPoint == null)
                        return;
                    
                    _foodFactory.CreateFood(orderData.FoodId, foodPoint).Forget();
                }
                
                await UniTask.Yield();
            }
        }

        public void Dispose()
        {
            _foodFactory?.Dispose();
            _foodTokenSource?.Dispose();
        }
    }
}