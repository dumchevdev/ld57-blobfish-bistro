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
using Game.Runtime._Game.Scripts.Runtime.Services.Save;
using Game.Runtime.CMS;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Kitchen
{
    public class KitchenService : IService, IDisposable
    {
        private readonly List<DinnerPointData> _foodPoints;
        private readonly ConcurrentQueue<string> _orderQueue;
        private readonly DinnerFactory _dinnerFactory = new();
        private readonly SemaphoreSlim _factoryLock = new(1, 1);

        private CancellationTokenSource _foodTokenSource;
    
        public KitchenService()
        {
            _foodPoints = new List<DinnerPointData>();
            _orderQueue = new ConcurrentQueue<string>();
        
            InitializeKitchen();
            InitializeTrash();

            ApplyFoodOrders().Forget();
        }

        private void InitializeKitchen()
        {
            var kitchenPrefab = CMSProvider.GetEntity(CMSPrefabs.Gameplay.Kitchen)
                .GetComponent<PrefabComponent>().Prefab;
            var kitchenObject = Object.Instantiate(kitchenPrefab);
            kitchenObject.name = nameof(KitchenService);
        
            var foodPoints = kitchenPrefab.GetComponentsInChildren<Transform>();
            for (int i = 2; i < foodPoints.Length; i++)
                _foodPoints.Add(new DinnerPointData(foodPoints[i]));
        }

        private static void InitializeTrash()
        {
            var trashPrefab = CMSProvider.GetEntity(CMSPrefabs.Gameplay.Trash)
                .GetComponent<PrefabComponent>().Prefab;
            var trashObject = Object.Instantiate(trashPrefab).GetComponent<TrashBehaviour>();
            trashObject.name = nameof(TrashBehaviour);
            trashObject.InteractionStrategy = new TrashInteraction();
            trashObject.Settings.IsClickable = true;
            trashObject.Settings.IsHighlightable = true;
        }

        public void Enqueue(string foodId)
        {
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

        private bool TryGetFreeFoodPoint(out DinnerPointData dinnerPoint)
        {
            foreach (var point in _foodPoints)
            {
                if (!point.IsOccupied)
                {
                    point.IsOccupied = true;
                    dinnerPoint = point;
                    return true;
                }
            }
            dinnerPoint = null;
            return false;
        }

        private bool AnyFoodPointFree()
        {
            return _foodPoints.Any(data => !data.IsOccupied);
        }

        private bool NeedCreateNewFood()
        {
            return _orderQueue.Count > 0 && AnyFoodPointFree() && !_dinnerFactory.IsOccupied;
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
                        await _dinnerFactory.CreateDinner(foodId, foodPoint);
                        ServicesProvider.GetService<SaveService>().UpdateStatisticsData(foodId);
                    }
                    catch
                    {
                        ReturnFoodPoint(foodPoint);
                        throw;
                    }
                }
            }
        }

        public void ReturnFoodPoint(DinnerPointData dinnerPointData)
        {
            dinnerPointData.IsOccupied = false;
        }

        public void ReturnFoodBehaviour(DinnerBehaviour dinnerBehaviour)
        {
            _dinnerFactory.ReturnFoodBehaviour(dinnerBehaviour);
        }

        public void Dispose()
        {
            _foodTokenSource?.Cancel();
            _foodTokenSource?.Dispose();
            _foodTokenSource = null;
            _factoryLock?.Dispose();
            _dinnerFactory?.Dispose();
        }
    }
}