using Cysharp.Threading.Tasks;
using Game.Runtime.Gameplay.Character;
using Game.Runtime.ServiceLocator;

namespace Game.Runtime.Gameplay.Interactives
{
    public class EmptyTableInteraction : IInteraction
    {
        public string DebugName => nameof(EmptyTableInteraction);

        public void ExecuteInteraction(InteractableObject interactable)
        {
            var tableData = ServiceLocator<TableService>.GetService().GetTable(interactable.Id);

            if (ServiceLocator<ClientsService>.GetService().IsFirstQueueClientSelected() && tableData.Client == null)
            {
                var clientData = ServiceLocator<ClientsService>.GetService().DequeueFirstClient();
                clientData.View.SetSelectedState(false);
                clientData.TableId = tableData.Id;
                tableData.Client = clientData;
                clientData.Movable.MoveToPoint(tableData.Behaviour.ClientPoint.position, 
                    () => clientData.State = ClientState.BrowsingMenu).Forget();
                
                return;
            }
            
            var tableBehaviour = interactable.GetComponent<TableBehaviour>();
            ServiceLocator<CharacterService>.GetService().MoveTo(tableBehaviour.CharacterPoint.position);
        }
    }
}