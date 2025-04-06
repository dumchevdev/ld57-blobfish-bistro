using Game.Runtime.Gameplay.Character;
using Game.Runtime.ServiceLocator;
using UnityEngine;

namespace Game.Runtime.Gameplay.Interactives
{
    public class OrderClientInteraction : IInteraction
    {
        public string DebugName => nameof(OrderClientInteraction);

        public void ExecuteInteraction(InteractableObject interactable)
        {
            var clientData = ServiceLocator<ClientsService>.GetService().GetClient(interactable.Id);
            var tableData = ServiceLocator<TableService>.GetService().GetTable(clientData.TableId);
            ServiceLocator<CharacterService>.GetService().MoveTo(tableData.Behaviour.CharacterPoint.position);
            ServiceLocator<CharacterService>.GetService().TakeOrder(tableData, () =>
            {
                tableData.Behaviour.SetStrategy(new WaitingForFoodTableInteraction());
                clientData.View.SetStrategy(new WaitingForFoodClientInteraction());
                clientData.State = ClientState.WaitingForFood;
            });
        }
    }
}