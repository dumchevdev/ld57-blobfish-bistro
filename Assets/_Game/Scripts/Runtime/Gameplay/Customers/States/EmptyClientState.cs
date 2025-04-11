using Game.Runtime._Game.Scripts.Runtime.Gameplay.Interactable;
using Game.Runtime._Game.Scripts.Runtime.StateMachine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers.States
{
    public class EmptyClientState : State<CustomerData>
    {
        public override void OnEnter()
        {
            Context.View.Settings.IsClickable = false;
            Context.View.Settings.IsHighlightable = false;
            Context.View.InteractionStrategy = new EmptyInteraction();
        }
    }
}