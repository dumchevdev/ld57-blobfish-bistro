using Game.Runtime.StateMachine;

namespace Game.Runtime.Gameplay.Interactives
{
    public class EmptyClientState : State<ClientData>
    {
        public override void OnEnter()
        {
            Context.View.Settings.IsClickable = false;
            Context.View.Settings.IsHighlightable = false;
            Context.View.InteractionStrategy = new EmptyClientInteraction();
        }
    }
}