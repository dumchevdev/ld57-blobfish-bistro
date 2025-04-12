using Cysharp.Threading.Tasks;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Interactable;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Services.Game;
using Game.Runtime._Game.Scripts.Runtime.StateMachine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers.States
{
    public class LeavingClientState : State<CustomerData>
    {
        public override void OnEnter()
        {
            Context.View.SetBlockFlipper(false);
            Context.View.ResetBehaviour();
            Context.View.InteractionStrategy = new EmptyInteraction();
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