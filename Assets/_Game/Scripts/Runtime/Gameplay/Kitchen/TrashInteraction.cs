using Game.Runtime._Game.Scripts.Runtime.Gameplay.Interactable;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Services.Game;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Kitchen
{
    public class TrashInteraction : IInteraction
    {
        private TrashBehaviour _trashBehaviour;
        
        public void ExecuteInteraction(InteractableObject interactable)
        {
            _trashBehaviour ??= interactable.GetComponent<TrashBehaviour>();
            ServicesProvider.GetService<GameService>().ResetCharacterHands(_trashBehaviour.CharacterPoint.position);
        }
    }
}