using Game.Runtime._Game.Scripts.Runtime.Gameplay.Interactable;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Kitchen;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Services.Game;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Dishes
{
    public class DinnerInteraction : IInteraction
    {
        private readonly string _foodId;
        private readonly DinnerBehaviour _dinnerBehaviour;
        private readonly DinnerPointData _dinnerPointData;

        public DinnerInteraction(string foodId, DinnerBehaviour dinnerBehaviour, DinnerPointData dinnerPointData)
        {
            _foodId = foodId;
            _dinnerBehaviour = dinnerBehaviour;
            _dinnerPointData = dinnerPointData;
        }

        public void ExecuteInteraction(InteractableObject interactable)
        {
            ServiceLocator<GameService>.GetService().TakeFood(_foodId, _dinnerBehaviour, _dinnerPointData);
        }
    }
}