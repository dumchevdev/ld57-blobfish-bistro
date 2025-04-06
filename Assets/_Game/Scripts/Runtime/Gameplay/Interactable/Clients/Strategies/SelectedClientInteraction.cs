using Game.Runtime.ServiceLocator;
using UnityEngine;

namespace Game.Runtime.Gameplay.Interactives
{
    public class SelectedClientInteraction : IInteraction
    {
        public string DebugName => nameof(SelectedClientInteraction);

        public void ExecuteInteraction(InteractableObject interactable)
        {
            var clientData = ServiceLocator<ClientsService>.GetService().GetClient(interactable.Id);
            Debug.Log($"Selected client: {clientData}. Id {interactable.Id}");
            clientData.State = ClientState.ChoosingTable;
            
            var clientBehaviour = interactable.GetComponent<ClientBehaviour>();
            clientBehaviour.SetSelectedState(true);
        }
    }
}