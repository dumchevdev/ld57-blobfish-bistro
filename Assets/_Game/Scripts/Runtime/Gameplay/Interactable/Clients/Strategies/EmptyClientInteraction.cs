namespace Game.Runtime.Gameplay.Interactives
{
    public class EmptyClientInteraction : IInteraction
    {
        public string DebugName => nameof(EmptyClientInteraction);

        public void ExecuteInteraction(InteractableObject interactable)
        {
            //NOTHING...
        }
    }
}