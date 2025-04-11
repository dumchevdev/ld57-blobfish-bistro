using Game.Runtime.Framework.Services.Game;
using Game.Runtime.ServiceLocator;

namespace Game.Runtime.Gameplay.Interactives
{
    public class PutFoodTableInteraction : IInteraction
    {
        public string DebugName => nameof(PutFoodTableInteraction);

        public void ExecuteInteraction(InteractableObject interactable)
        {
            var gameService = ServiceLocator<GameService>.GetService();
            var orderData = gameService.GetOrderByTable(interactable.Id);
            if (orderData != null) gameService.PutOrderFood(orderData);
        }
    }
}