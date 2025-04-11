using Cysharp.Threading.Tasks;
using Game.Runtime.Framework.Services.Game;
using Game.Runtime.ServiceLocator;
using Game.Runtime.StateMachine;

namespace Game.Runtime.Gameplay.Interactives
{
    public class LeavingClientState : State<ClientData>
    {
        public override void OnEnter()
        {
            Context.View.ResetBehaviour();
            Context.View.InteractionStrategy = new EmptyClientInteraction();
            Context.View.Settings.IsClickable = false;
            Context.View.Settings.IsHighlightable = false;
            var gameService = ServiceLocator<GameService>.GetService();
            var orderData = gameService.GetOrderByClient(Context.Id);
            if (orderData != null)
            {
                orderData.IsClosed = true;
                gameService.RemoveOrder(orderData);
            }
            gameService.RemoveClient(Context).Forget();
        }
    }
}