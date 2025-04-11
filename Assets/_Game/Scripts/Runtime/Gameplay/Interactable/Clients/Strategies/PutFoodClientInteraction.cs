using Game.Runtime.Framework.Services.Game;
using Game.Runtime.ServiceLocator;

namespace Game.Runtime.Gameplay.Interactives
{
    public class PutFoodClientInteraction : IInteraction
    {
        public string DebugName => nameof(PutFoodClientInteraction);

        public void ExecuteInteraction(InteractableObject interactable)
        {
            var gameService = ServiceLocator<GameService>.GetService();
            var orderData = gameService.GetOrderByClient(interactable.Id);
            if (orderData != null) gameService.PutOrderFood(orderData);
        }
    }
}