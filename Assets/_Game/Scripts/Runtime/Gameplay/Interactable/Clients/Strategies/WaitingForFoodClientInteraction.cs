using Game.Runtime.Gameplay.Character;
using Game.Runtime.ServiceLocator;
using UnityEngine;

namespace Game.Runtime.Gameplay.Interactives
{
    public class WaitingForFoodClientInteraction : IInteraction
    {
        public string DebugName => nameof(WaitingForFoodClientInteraction);

        public void ExecuteInteraction(InteractableObject interactable)
        {
            var clientData = ServiceLocator<ClientsService>.GetService().GetClient(interactable.Id);
            var tableData = ServiceLocator<TableService>.GetService().GetTable(clientData.TableId);
            var characterService = ServiceLocator<CharacterService>.GetService();
            
            characterService.MoveTo(tableData.Behaviour.CharacterPoint.position, () =>
            {
                if (characterService.TryGetHandWithFood(tableData.Client.OrderData.FoodId, out var handData))
                {
                    tableData.Food = handData.FoodData;
                    
                    handData.FoodData.Behaviour.transform.position = tableData.Behaviour.FoodPoint.position;
                    handData.FoodData.Behaviour.gameObject.SetActive(true);
                    handData.FoodData = null;
                    
                    tableData.Behaviour.SetStrategy(new EatingFoodTableInteraction());
                    tableData.Client.View.SetStrategy(new EatingFoodClientInteraction());
                    tableData.Client.State = ClientState.EatingFood;
                }
            });
        }
    }
}