using Game.Runtime._Game.Scripts.Runtime.Gameplay.Interactable;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Services.Game;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Tables.Strategies
{
    public class CleanupTableInteraction : IInteraction
    {
        public string DebugName => nameof(CleanupTableInteraction);

        public void ExecuteInteraction(InteractableObject interactable)
        {
            var gameService = ServiceLocator<GameService>.GetService();
            var orderData = gameService.GetOrderByTable(interactable.Id);
            if (orderData != null) gameService.CleanupTable(orderData);
        }
    }
}