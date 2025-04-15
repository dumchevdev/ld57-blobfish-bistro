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
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Kitchen
{
    public class DinnerFactory : IDisposable
    {
        public bool IsOccupied { get; private set; }
        
        private readonly List<DinnerBehaviour> _dinnerPool = new();
        private CancellationTokenSource _factoryTokenSource;

        public async UniTask CookingDinner(CookingData cookingData)
        {
            _factoryTokenSource = new CancellationTokenSource();

            try
            {
                IsOccupied = true;
                cookingData.DinnerBehaviour.StartCookingTimer(cookingData.Model.CookingTime);

                await UniTask.WaitForSeconds(cookingData.Model.CookingTime, cancellationToken: _factoryTokenSource.Token);
                
                cookingData.DinnerBehaviour.SetFishingState();
                cookingData.DinnerBehaviour.EnableInteraction();
                
                cookingData.DinnerBehaviour.Settings.IsClickable = true;
                cookingData.DinnerBehaviour.Settings.IsHighlightable = true;
                cookingData.DinnerBehaviour.InteractionStrategy = new DinnerInteraction(cookingData);
            }
            finally
            {
                IsOccupied = false;
                ResetFactoryToken();
            }
        }

        public (DinnerComponent, DinnerBehaviour) CreateDinner(string foodId, DinnerPointData dinnerPointData)
        {
            var foodsComponent = CMSProvider.GetEntity(CMSPrefabs.Gameplay.DishesLibrary).GetComponent<DishesLibraryComponent>();
            var foodComponent = foodsComponent.Dishes.First(food => food.Id == foodId);

            var foodBehaviour = GetFoodBehaviour();
                
            foodBehaviour.transform.position = dinnerPointData.Point.position;
            foodBehaviour.SetFoodSprite(foodComponent.Sprite);
            foodBehaviour.SetQueueState();
            
            return (foodComponent, foodBehaviour);
        }

        private DinnerBehaviour GetFoodBehaviour()
        {
            DinnerBehaviour dinnerBehaviour = null;
            
            for (int i = 0; i < _dinnerPool.Count; i++)
            {
                if (!_dinnerPool[i].gameObject.activeInHierarchy)
                {
                    dinnerBehaviour = _dinnerPool[i];
                    _dinnerPool.RemoveAt(i);
                    return dinnerBehaviour;
                }
            }
         
            var foodPrefab = CMSProvider.GetEntity(CMSPrefabs.Gameplay.DinnerBehaviour).GetComponent<PrefabComponent>().Prefab;
            dinnerBehaviour = Object.Instantiate(foodPrefab).GetComponent<DinnerBehaviour>();

            return dinnerBehaviour;
        }

        public void ReturnFoodBehaviour(DinnerBehaviour dinnerBehaviour)
        {
            dinnerBehaviour.Settings.IsClickable = false;
            dinnerBehaviour.Settings.IsHighlightable = false;
            dinnerBehaviour.gameObject.SetActive(false);
            _dinnerPool.Add(dinnerBehaviour);
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