using Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers.Strategies;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Interactable;
using Game.Runtime._Game.Scripts.Runtime.StateMachine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers.States
{
    public class WaitingInQueueClientState : State<CustomerData>
    {
        public override void OnEnter()
        {
            Context.View.Settings.IsClickable = true;
            Context.View.Settings.IsHighlightable = true;
            Context.View.InteractionStrategy = new SelectedCustomerInteraction();
        }

        public override void OnExit()
        {
            Context.View.InteractionStrategy = new EmptyInteraction();
        }
    }
}