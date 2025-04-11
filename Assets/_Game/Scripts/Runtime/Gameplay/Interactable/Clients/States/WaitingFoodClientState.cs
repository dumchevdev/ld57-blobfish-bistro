using Game.Runtime.Framework.Services.Game;
using Game.Runtime.ServiceLocator;
using Game.Runtime.StateMachine;

namespace Game.Runtime.Gameplay.Interactives
{
    public class WaitingFoodClientState : State<ClientData>
    {
        public override void OnEnter()
        {
            Context.MoodChecker.ResetMoodTimer();
            var orderData = ServiceLocator<GameService>.GetService().GetOrderByClient(Context.Id);
            Context.View.ShowHint(true, orderData.FoodId);
            Context.View.InteractionStrategy = new PutFoodClientInteraction();
        }

        public override void OnExit()
        {
            Context.View.ShowHint(false);
        }
    }
}