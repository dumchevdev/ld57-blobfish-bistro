using Cysharp.Threading.Tasks;
using Game.Runtime.Framework.Services.Game;
using Game.Runtime.Gameplay.Character;
using Game.Runtime.ServiceLocator;

namespace Game.Runtime.Gameplay.Interactives
{
    public class MoveToTableInteraction : IInteraction
    {
        public string DebugName => nameof(MoveToTableInteraction);

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