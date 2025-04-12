using Cysharp.Threading.Tasks;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Interactable;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Services.Game;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Tables.Strategies
{
    public class MoveToTableInteraction : IInteraction
    {
        public void ExecuteInteraction(InteractableObject interactable)
        {
            var gameService = ServiceLocator<GameService>.GetService();
            var orderData = gameService.GetOrderByTable(interactable.Id);
            if (gameService.IsSelectedClient && orderData == null)
            {
                ServiceLocator<GameService>.GetService().PutQueueClientAtTable(interactable.Id).Forget();
                return;
            }
            
            var tableBehaviour = interactable.GetComponent<TableBehaviour>();
            gameService.MoveCharacter(tableBehaviour.CharacterPoint.position);
        }
    }
}