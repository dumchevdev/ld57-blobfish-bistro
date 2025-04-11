using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime._Game.Scripts.Runtime.CMS;
using Game.Runtime._Game.Scripts.Runtime.CMS.Components.Commons;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Dishes;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime.CMS;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Kitchen
{
    public class KitchenService : IService, IDisposable
    {
        private readonly List<FoodPointData> _foodPoints;
        private readonly ConcurrentQueue<string> _orderQueue;
        private readonly FoodFactory _foodFactory;
        private readonly SemaphoreSlim _factoryLock = new(1, 1);

        private CancellationTokenSource _foodTokenSource;
    
        public KitchenService()
        {
            _foodPoints = new List<FoodPointData>();
            _orderQueue = new ConcurrentQueue<string>();
        
            var deliveryPrefab = CMSProvider.GetEntity(CMSPrefabs.Gameplay.FoodDeliveryArea)
                .GetComponent<PrefabComponent>().Prefab;
            var deliveryObject = Object.Instantiate(deliveryPrefab);
            deliveryObject.name = nameof(KitchenService);
        
            _foodFactory = new FoodFactory();

            var foodPoints = deliveryPrefab.GetComponentsInChildren<Transform>();
            for (int i = 2; i < foodPoints.Length; i++)
                _foodPoints.Add(new FoodPointData(foodPoints[i]));

            ApplyFoodOrders().Forget();
        }

        public void Enqueue(string foodId)
        {
            Debug.Log($"[FOOD DELIVERY] Enqueue foodId: {foodId}");
            _orderQueue.Enqueue(foodId);
        }

        private bool TryDequeue(out string foodId)
        {
            if (_orderQueue.Count > 0)
            {
                _orderQueue.TryDequeue(out foodId);
                return true;
            }
            foodId = null;
            return false;
        }

        private bool TryGetFreeFoodPoint(out FoodPointData foodPoint)
        {
            foreach (var point in _foodPoints)
            {
                if (!point.IsOccupied)
                {
                    point.IsOccupied = true;
                    foodPoint = point;
                    return true;
                }
            }
            foodPoint = null;
            return false;
        }

        private bool AnyFoodPointFree()
        {
            return _foodPoints.Any(data => !data.IsOccupied);
        }

        private bool NeedCreateNewFood()
        {
            return _orderQueue.Count > 0 && AnyFoodPointFree();
        }
    
        private async UniTask ApplyFoodOrders()
        {
            _foodTokenSource = new CancellationTokenSource();
        
            while (_foodTokenSource != null && !_foodTokenSource.IsCancellationRequested)
            {
                await UniTask.WaitUntil(NeedCreateNewFood, cancellationToken: _foodTokenSource.Token);

                if (TryDequeue(out var foodId) && TryGetFreeFoodPoint(out var foodPoint))
                {
                    try
                    {
                        Debug.Log($"[FOOD DELIVERY] Creating food order for foodId: {foodId}");
                        await _foodFactory.CreateFood(foodId, foodPoint);
                    }
                    catch
                    {
                        ReturnFoodPoint(foodPoint);
                        throw;
                    }
                }
            }
        }

        public void ReturnFoodPoint(FoodPointData foodPointData)
        {
            foodPointData.IsOccupied = false;
        }

        public void ReturnFoodBehaviour(DinnerBehaviour dinnerBehaviour)
        {
            _foodFactory.ReturnFoodBehaviour(dinnerBehaviour);
        }

        public void Dispose()
        {
            _foodTokenSource?.Cancel();
            _foodTokenSource?.Dispose();
            _foodTokenSource = null;
            _factoryLock?.Dispose();
            _foodFactory?.Dispose();
        }
    }
}