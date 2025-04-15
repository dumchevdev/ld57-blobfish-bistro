using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Interactable
{
    public class EmptyInteraction : IInteraction
    {
        public void ExecuteInteraction(InteractableObject interactable)
        {
            Debug.Log("[GAMEPLAY] Empty Interaction");
        }
    }
}