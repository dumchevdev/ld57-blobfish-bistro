using Game.Runtime.Gameplay.Character;
using Game.Runtime.ServiceLocator;
using UnityEngine;

namespace Game.Runtime.Gameplay.Interactives
{
    public class EatingFoodTableInteraction : IInteraction
    {
        public string DebugName => nameof(EatingFoodTableInteraction);

        public void ExecuteInteraction(InteractableObject interactable)
        {
            var tableBehaviour = interactable.GetComponent<TableBehaviour>();
            ServiceLocator<CharacterService>.GetService().MoveTo(tableBehaviour.CharacterPoint.position);
        }
    }
}