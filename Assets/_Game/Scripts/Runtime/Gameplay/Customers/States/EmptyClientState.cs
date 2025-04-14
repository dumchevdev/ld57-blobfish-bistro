using Game.Runtime._Game.Scripts.Runtime.Gameplay.Interactable;
using Game.Runtime._Game.Scripts.Runtime.StateMachine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers.States
{
    public class EmptyClientState : State<CustomerData>
    {
        public override void OnEnter()
        {
            Context.Behaviour.Settings.IsClickable = false;
            Context.Behaviour.Settings.IsHighlightable = false;
            Context.Behaviour.InteractionStrategy = new EmptyInteraction();
        }
    }
}