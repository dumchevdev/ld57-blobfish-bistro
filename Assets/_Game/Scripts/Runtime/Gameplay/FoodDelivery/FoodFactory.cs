using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Commons;
using Game.Runtime.CMS.Components.Gameplay;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Runtime.Gameplay.FoodDelivery
{
    public class FoodFactory : IDisposable
    {
        public bool IsFree;
        
        private readonly List<FoodBehaviour> _foodPool;
        
        private CancellationTokenSource _factoryTokenSource;

        public FoodFactory()
        {
            _foodPool = new List<FoodBehaviour>();
            IsFree = true;
        }

        public async UniTask CreateFood(string foodId, FoodPointData foodPointData)
        {
            _factoryTokenSource?.Cancel();
            _factoryTokenSource = new CancellationTokenSource();

            try
            {
                var foodsComponent = CMSProvider.GetEntity(CMSPrefabs.Gameplay.Foods).GetComponent<FoodsComponent>();
                var foodComponent = foodsComponent.Foods.First(food => food.Id == foodId);

                var foodBehaviour = GetFoodBehaviour();
                IsFree = false;

                await UniTask.WaitForSeconds(foodComponent.CookingTime, cancellationToken: _factoryTokenSource.Token)
                    .SuppressCancellationThrow();
                
                if (_factoryTokenSource == null && _factoryTokenSource.IsCancellationRequested)
                    return;
                
                foodBehaviour.SetFoodSprite(foodComponent.Sprite);
                foodBehaviour.transform.position = foodPointData.Point.position;
                foodBehaviour.SetStrategy(new FoodInteraction(foodId, foodBehaviour, foodPointData));
                foodBehaviour.gameObject.SetActive(true);
                
                IsFree = true;
            }
            finally
            {
                ResetFactoryToken();
            }
        }

        private FoodBehaviour GetFoodBehaviour()
        {
            FoodBehaviour foodBehaviour = null;
            
            if (_foodPool.Count > 0)
            {
                foodBehaviour = _foodPool[0];
                _foodPool.Remove(foodBehaviour);

                return foodBehaviour;
            }
            
            var foodPrefab = CMSProvider.GetEntity(CMSPrefabs.Gameplay.FoodBehaviour).GetComponent<PrefabComponent>().Prefab;
            foodBehaviour = Object.Instantiate(foodPrefab).GetComponent<FoodBehaviour>();

            return foodBehaviour;
        }

        public void ReturnFoodBehaviour(FoodBehaviour foodBehaviour)
        {
            foodBehaviour.gameObject.SetActive(false);
            _foodPool.Add(foodBehaviour);
        }

        private void ResetFactoryToken()
        {
            _factoryTokenSource?.Dispose();
            _factoryTokenSource = null;
        }

        public void Dispose()
        {
            ResetFactoryToken();
        }
    }
}