using Game.Runtime._Game.Scripts.Runtime.Gameplay.Interactable;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Services.Game;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers.Strategies
{
    public class SelectedCustomerInteraction : IInteraction
    {
        public void ExecuteInteraction(InteractableObject interactable)
        {
            ServiceLocator<GameService>.GetService().SelectQueueClient(interactable.Id);
        }
    }
}