using Game.Runtime.Gameplay.Character;
using Game.Runtime.ServiceLocator;
using UnityEngine;

namespace Game.Runtime.Gameplay.Interactives
{
    public class OrderTableInteraction : IInteraction
    {
        public string DebugName => nameof(OrderTableInteraction);

        public void ExecuteInteraction(InteractableObject interactable)
        {
            var tableData = ServiceLocator<TableService>.GetService().GetTable(interactable.Id);
            var tableBehaviour = interactable.GetComponent<TableBehaviour>();
            
            ServiceLocator<CharacterService>.GetService().MoveTo(tableBehaviour.CharacterPoint.position);
            ServiceLocator<CharacterService>.GetService().TakeOrder(tableData, () =>
            {
                tableData.Behaviour.SetStrategy(new WaitingForFoodTableInteraction());
                tableData.Client.View.SetStrategy(new WaitingForFoodClientInteraction());
                tableData.Client.State = ClientState.WaitingForFood;
            });
        }
    }
}