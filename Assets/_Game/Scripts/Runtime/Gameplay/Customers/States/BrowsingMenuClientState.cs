using Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers.Strategies;
using Game.Runtime._Game.Scripts.Runtime.StateMachine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers.States
{
    public class BrowsingMenuClientState : State<CustomerData>
    {
        public override void OnEnter()
        {
            Context.MoodChecker.ResetMoodTimer();
            Context.Behaviour.Settings.IsClickable = true;
            Context.Behaviour.Settings.IsHighlightable = true;
            Context.Behaviour.InteractionStrategy = new MoveToCustomerInteraction();
        }
    }
}