using Game.Runtime._Game.Scripts.Runtime.Gameplay.Interactable;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Services.Game;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Tables.Strategies
{
    public class TakeOrderTableInteraction : IInteraction
    {
        public string DebugName => nameof(TakeOrderTableInteraction);

        public void ExecuteInteraction(InteractableObject interactable)
        {
            var gameService = ServicesProvider.GetService<GameService>();
            var orderData = gameService.GetOrderByTable(interactable.Id);
            
            if (orderData != null && !orderData.OrderAlreadyTaken)
                gameService.TakeOrder(orderData);
        }
    }
}