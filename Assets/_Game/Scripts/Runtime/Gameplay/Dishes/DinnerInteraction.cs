using Game.Runtime._Game.Scripts.Runtime.Gameplay.Interactable;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Kitchen;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Services.Game;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Dishes
{
    public class DinnerInteraction : IInteraction
    {
        private readonly CookingData _cookingData;

        public DinnerInteraction(CookingData cookingData)
        {
            _cookingData = cookingData;
        }

        public void ExecuteInteraction(InteractableObject interactable)
        {
            ServicesProvider.GetService<GameService>().TakeFood(_cookingData);
        }
    }
}