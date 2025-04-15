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
using Game.Runtime._Game.Scripts.Runtime.Services.Audio;
using Game.Runtime._Game.Scripts.Runtime.Services.Game;
using Game.Runtime._Game.Scripts.Runtime.Services.Save;
using Game.Runtime.CMS;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Kitchen
{
    public class KitchenService : IService, IDisposable
    {
        private readonly List<DinnerPointData> _dinnerPoints;
        private readonly Queue<CookingData> _needCooking;
        private readonly ConcurrentQueue<OrderData> _orderQueue;
        private readonly DinnerFactory _dinnerFactory = new();
        private readonly SemaphoreSlim _factoryLock = new(1, 1);
        
        private CancellationTokenSource _foodTokenSource;
    
        public KitchenService()
        {
            _dinnerPoints = new List<DinnerPointData>();
            _needCooking = new Queue<CookingData>();
            _orderQueue = new ConcurrentQueue<OrderData>();
        
            InitializeKitchen();
            InitializeTrash();

            ApplyOrders().Forget();
        }

        private void InitializeKitchen()
        {
            var kitchenPrefab = CMSProvider.GetEntity(CMSPrefabs.Gameplay.Kitchen)
                .GetComponent<PrefabComponent>().Prefab;
            var kitchenObject = Object.Instantiate(kitchenPrefab);
            kitchenObject.name = nameof(KitchenService);
        
            var foodPoints = kitchenPrefab.GetComponentsInChildren<Transform>();
            for (int i = 2; i < foodPoints.Length; i++)
                _dinnerPoints.Add(new DinnerPointData(foodPoints[i]));
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

        public void Enqueue(OrderData orderData)
        {
            _orderQueue.Enqueue(orderData);
        }
        
        private bool AnyFreeDinnerPoints() => _dinnerPoints.Any(data => !data.IsOccupied);

        private DinnerPointData GetFreeDinnerPoint()
        {
            foreach (var point in _dinnerPoints)
            {
                if (!point.IsOccupied)
                {
                    point.IsOccupied = true;
                    return point;
                }
            }
            
            return null;
        }
    
        private async UniTask ApplyOrders()
        {
            _foodTokenSource = new CancellationTokenSource();


            while (_foodTokenSource != null && !_foodTokenSource.IsCancellationRequested)
            {
                await UniTask.Yield(cancellationToken: _foodTokenSource.Token, cancelImmediately: true);

                if (AnyFreeDinnerPoints() && _orderQueue.TryDequeue(out OrderData orderData))
                {
                    PreviewOrder(orderData.DinnerId, GetFreeDinnerPoint());
                }

                CookDinner().Forget();
            }
        }

        private async UniTask CookDinner()
        {
            if (!_dinnerFactory.IsOccupied && _needCooking.TryDequeue(out CookingData cookingData))
            {
                try
                {
                    await _dinnerFactory.CookingDinner(cookingData);
                    ServicesProvider.GetService<AudioService>().Play(CMSPrefabs.Audio.SFX.SFXBubble);
                    ServicesProvider.GetService<SaveService>().UpdateStatisticsData(cookingData.Model.Id);
                }
                catch
                {
                    ReturnFoodPoint(cookingData.DinnerPointData);
                }
            }
        }

        private void PreviewOrder(string dinnerId, DinnerPointData dinnerPoint)
        {
            var dinnerBehaviour = _dinnerFactory.CreateDinner(dinnerId, dinnerPoint);
            _needCooking.Enqueue(new CookingData(dinnerBehaviour.Item1, dinnerBehaviour.Item2, dinnerPoint));
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