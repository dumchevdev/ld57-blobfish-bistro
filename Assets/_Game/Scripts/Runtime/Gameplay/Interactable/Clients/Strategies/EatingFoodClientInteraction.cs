using Game.Runtime.Gameplay.Character;
using Game.Runtime.ServiceLocator;

namespace Game.Runtime.Gameplay.Interactives
{
    public class EatingFoodClientInteraction : IInteraction
    {
        public string DebugName => nameof(EatingFoodClientInteraction);

        public void ExecuteInteraction(InteractableObject interactable)
        {
            var clientData = ServiceLocator<ClientsService>.GetService().GetClient(interactable.Id);
            var tableData = ServiceLocator<TableService>.GetService().GetTable(clientData.TableId);
            ServiceLocator<CharacterService>.GetService().MoveTo(tableData.Behaviour.CharacterPoint.position);
            clientData.View.ShowHint(false);
        }
    }
}