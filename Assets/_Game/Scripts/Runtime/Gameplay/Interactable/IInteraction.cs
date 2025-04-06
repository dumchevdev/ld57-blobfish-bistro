namespace Game.Runtime.Gameplay.Interactives
{
    public interface IInteraction
    {
        string DebugName { get; }
        void ExecuteInteraction(InteractableObject interactable);
    }
}