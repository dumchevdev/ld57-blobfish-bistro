using Cysharp.Threading.Tasks;
using Game.Runtime.Framework.Services.Game;
using Game.Runtime.Gameplay.Interactives;
using Game.Runtime.ServiceLocator;

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
            ServiceLocator<GameService>.GetService().TakeFood(_foodId, _foodBehaviour, _foodPointData);
        }
    }
}