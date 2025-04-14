using Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers.Strategies;
using Game.Runtime._Game.Scripts.Runtime.StateMachine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers.States
{
    public class WaitingOrderClientState : State<CustomerData>
    {
        public override void OnEnter()
        {
            Context.MoodChecker.ResetMoodTimer();
            Context.Behaviour.ShowHintWarning(true);
            Context.Behaviour.InteractionStrategy = new TakeOrderCustomerInteraction();
        }
        
        public override void OnExit()
        {
            Context.Behaviour.ShowHintWarning(false);
        }
    }
}