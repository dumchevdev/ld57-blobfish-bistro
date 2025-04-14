using Game.Runtime._Game.Scripts.Runtime.Gameplay.Interactable;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Services.Game;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers.Strategies
{
    public class ServingDinnerCustomerInteraction : IInteraction
    {
        public void ExecuteInteraction(InteractableObject interactable)
        {
            var gameService = ServicesProvider.GetService<GameService>();
            var orderData = gameService.GetOrderByClient(interactable.Id);
            if (orderData != null) gameService.PutOrderFood(orderData);
        }
    }
}