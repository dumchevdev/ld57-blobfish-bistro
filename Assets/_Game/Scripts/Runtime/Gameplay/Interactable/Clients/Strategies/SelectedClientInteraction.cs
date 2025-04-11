using Game.Runtime.Framework.Services.Game;
using Game.Runtime.ServiceLocator;

namespace Game.Runtime.Gameplay.Interactives
{
    public class SelectedClientInteraction : IInteraction
    {
        public string DebugName => nameof(SelectedClientInteraction);

        public void ExecuteInteraction(InteractableObject interactable)
        {
            ServiceLocator<GameService>.GetService().SelectQueueClient(interactable.Id);
        }
    }
}