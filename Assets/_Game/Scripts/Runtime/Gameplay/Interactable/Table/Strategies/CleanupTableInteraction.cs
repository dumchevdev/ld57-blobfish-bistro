using Game.Runtime.Gameplay.Character;
using Game.Runtime.Gameplay.FoodDelivery;
using Game.Runtime.ServiceLocator;
using UnityEngine;

namespace Game.Runtime.Gameplay.Interactives
{
    public class CleanupTableInteraction : IInteraction
    {
        public string DebugName => nameof(CleanupTableInteraction);

        public void ExecuteInteraction(InteractableObject interactable)
        {
            var tableData = ServiceLocator<TableService>.GetService().GetTable(interactable.Id);
            if (tableData.Food != null)
            {
                ServiceLocator<CharacterService>.GetService().MoveTo(tableData.Behaviour.CharacterPoint.position, () =>
                {
                    ServiceLocator<FoodDeliveryService>.GetService().ReturnFoodToPool(tableData.Food.Behaviour);
                    
                    tableData.Food = null;
                    tableData.Client = null;
                    tableData.Behaviour.SetStrategy(new EmptyTableInteraction());
                });
            }
        }
    }
}