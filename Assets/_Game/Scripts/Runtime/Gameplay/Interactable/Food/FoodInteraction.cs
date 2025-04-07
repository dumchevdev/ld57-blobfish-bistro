using Game.Runtime.Gameplay.Character;
using Game.Runtime.Gameplay.Interactives;
using Game.Runtime.ServiceLocator;
using UnityEngine;

namespace Game.Runtime.Gameplay.FoodDelivery
{
    public class FoodInteraction : IInteraction
    {
        private readonly string _foodId;
        private readonly FoodBehaviour _foodBehaviour;
        private readonly FoodPointData _foodPointData;

        public FoodInteraction(string foodId, FoodBehaviour foodBehaviour, FoodPointData foodPointData)
        {
            _foodId = foodId;
            _foodBehaviour = foodBehaviour;
            _foodPointData = foodPointData;
        }

        public string DebugName => nameof(FoodInteraction);

        public void ExecuteInteraction(InteractableObject interactable)
        {
            if (!_foodBehaviour.Interacting) return;
            
            var foodBehaviour = interactable.GetComponent<FoodBehaviour>();
            ServiceLocator<CharacterService>.GetService().MoveTo(foodBehaviour.Point.position, () =>
            {
                var characterHand = ServiceLocator<CharacterService>.GetService().GetFreeHand();
                if (characterHand != null)
                {
                    _foodBehaviour.gameObject.SetActive(false);
                    characterHand.FoodData = new FoodData(_foodId, _foodBehaviour);
                    ServiceLocator<FoodDeliveryService>.GetService().ReturnFoodPoint(_foodPointData);
                }
            });
        }
    }
}