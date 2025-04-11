using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime._Game.Scripts.Runtime.CMS;
using Game.Runtime._Game.Scripts.Runtime.CMS.Components.Commons;
using Game.Runtime._Game.Scripts.Runtime.CMS.Components.Gameplay;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Dishes;
using Game.Runtime.CMS;
using Object = UnityEngine.Object;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Kitchen
{
    public class FoodFactory : IDisposable
    {
        private readonly List<DinnerBehaviour> _foodPool = new();
        private CancellationTokenSource _factoryTokenSource;

        public async UniTask CreateFood(string foodId, FoodPointData foodPointData)
        {
            _factoryTokenSource?.Cancel();
            _factoryTokenSource = new CancellationTokenSource();

            try
            {
                var foodsComponent = CMSProvider.GetEntity(CMSPrefabs.Gameplay.Foods).GetComponent<FoodsComponent>();
                var foodComponent = foodsComponent.Foods.First(food => food.Id == foodId);

                var foodBehaviour = GetFoodBehaviour();

                await UniTask.WaitForSeconds(foodComponent.CookingTime, cancellationToken: _factoryTokenSource.Token);
                
                foodBehaviour.SetFoodSprite(foodComponent.Sprite);
                foodBehaviour.transform.position = foodPointData.Point.position;

                foodBehaviour.Settings.IsClickable = true;
                foodBehaviour.Settings.IsHighlightable = true;
                foodBehaviour.InteractionStrategy = new DinnerInteraction(foodId, foodBehaviour, foodPointData);
                foodBehaviour.gameObject.SetActive(true);
            }
            finally
            {
                ResetFactoryToken();
            }
        }

        private DinnerBehaviour GetFoodBehaviour()
        {
            DinnerBehaviour dinnerBehaviour = null;
            
            if (_foodPool.Count > 0)
            {
                dinnerBehaviour = _foodPool[0];
                _foodPool.Remove(dinnerBehaviour);

                return dinnerBehaviour;
            }
            
            var foodPrefab = CMSProvider.GetEntity(CMSPrefabs.Gameplay.FoodBehaviour).GetComponent<PrefabComponent>().Prefab;
            dinnerBehaviour = Object.Instantiate(foodPrefab).GetComponent<DinnerBehaviour>();

            return dinnerBehaviour;
        }

        public void ReturnFoodBehaviour(DinnerBehaviour dinnerBehaviour)
        {
            dinnerBehaviour.Settings.IsClickable = false;
            dinnerBehaviour.Settings.IsHighlightable = false;
            dinnerBehaviour.gameObject.SetActive(false);
            _foodPool.Add(dinnerBehaviour);
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