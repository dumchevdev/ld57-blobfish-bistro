using Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers.Strategies;
using Game.Runtime._Game.Scripts.Runtime.StateMachine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers.States
{
    public class BrowsingMenuClientState : State<CustomerData>
    {
        public override void OnEnter()
        {
            Context.MoodChecker.ResetMoodTimer();
            Context.View.Settings.IsClickable = true;
            Context.View.Settings.IsHighlightable = true;
            Context.View.InteractionStrategy = new MoveToCustomerInteraction();
        }
    }
}