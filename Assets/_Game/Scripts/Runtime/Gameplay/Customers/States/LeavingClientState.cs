using Cysharp.Threading.Tasks;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Interactable;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Services.Audio;
using Game.Runtime._Game.Scripts.Runtime.Services.Game;
using Game.Runtime._Game.Scripts.Runtime.StateMachine;
using Game.Runtime.CMS;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers.States
{
    public class LeavingClientState : State<CustomerData>
    {
        public override void OnEnter()
        {
            ServicesProvider.GetService<AudioService>().Play(CMSPrefabs.Audio.SFX.SFXBubble);
            Context.Behaviour.SetBlockFlipper(false);
            Context.Behaviour.ResetBehaviour();
            Context.Behaviour.InteractionStrategy = new EmptyInteraction();
            Context.Behaviour.Settings.IsClickable = false;
            Context.Behaviour.Settings.IsHighlightable = false;
            var gameService = ServicesProvider.GetService<GameService>();
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