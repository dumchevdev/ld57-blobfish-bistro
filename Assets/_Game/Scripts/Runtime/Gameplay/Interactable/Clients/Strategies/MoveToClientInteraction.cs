using Game.Runtime.Framework.Services.Game;
using Game.Runtime.ServiceLocator;

namespace Game.Runtime.Gameplay.Interactives
{
    public class MoveToClientInteraction : IInteraction
    {
        public string DebugName => nameof(MoveToClientInteraction);

        public void ExecuteInteraction(InteractableObject interactable)
        {
            var gameService = ServiceLocator<GameService>.GetService();
            var orderData = gameService.GetOrderByClient(interactable.Id);
            gameService.MoveCharacter(orderData.TableData.Behaviour.CharacterPoint.position);
        }
    }
}