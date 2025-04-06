using UnityEngine;

namespace Game.Runtime.Gameplay.Interactives
{
    public class NotingClientInteraction : IInteraction
    {
        public string DebugName => nameof(NotingClientInteraction);

        public void ExecuteInteraction(InteractableObject interactable)
        {
        }
    }
}