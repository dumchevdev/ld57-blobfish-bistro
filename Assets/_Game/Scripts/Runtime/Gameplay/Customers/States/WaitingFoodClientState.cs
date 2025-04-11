using Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers.Strategies;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Services.Game;
using Game.Runtime._Game.Scripts.Runtime.StateMachine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers.States
{
    public class WaitingFoodClientState : State<CustomerData>
    {
        public override void OnEnter()
        {
            Context.MoodChecker.ResetMoodTimer();
            var orderData = ServiceLocator<GameService>.GetService().GetOrderByClient(Context.Id);
            Context.View.ShowHint(true, orderData.FoodId);
            Context.View.InteractionStrategy = new ServingDinnerCustomerInteraction();
        }

        public override void OnExit()
        {
            Context.View.ShowHint(false);
        }
    }
}