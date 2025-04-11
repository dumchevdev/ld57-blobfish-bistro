using Game.Runtime.Framework.Services.Game;
using Game.Runtime.ServiceLocator;

namespace Game.Runtime.Gameplay.Interactives
{
    public class TakeOrderTableInteraction : IInteraction
    {
        public string DebugName => nameof(TakeOrderTableInteraction);

        public void ExecuteInteraction(InteractableObject interactable)
        {
            var gameService = ServiceLocator<GameService>.GetService();
            var orderData = gameService.GetOrderByTable(interactable.Id);
            if (orderData != null) gameService.TakeOrder(orderData);
        }
    }
}