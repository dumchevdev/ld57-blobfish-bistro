using Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers.Strategies;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Interactable;
using Game.Runtime._Game.Scripts.Runtime.StateMachine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers.States
{
    public class WaitingInQueueClientState : State<CustomerData>
    {
        public override void OnEnter()
        {
            Context.Behaviour.Settings.IsClickable = true;
            Context.Behaviour.Settings.IsHighlightable = true;
            Context.Behaviour.InteractionStrategy = new SelectedCustomerInteraction();
        }

        public override void OnExit()
        {
            Context.MoodChecker.ResetMoodTimer();
            Context.Behaviour.InteractionStrategy = new EmptyInteraction();
        }
    }
}