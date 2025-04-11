using Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers.Strategies;
using Game.Runtime._Game.Scripts.Runtime.StateMachine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers.States
{
    public class EatingOrderFoodClientState : State<CustomerData>
    {
        public override void OnEnter()
        {
            Context.MoodChecker.StopMoodTimer();
            Context.View.InteractionStrategy = new MoveToCustomerInteraction();
        }
    }
}