using Game.Runtime.Gameplay.Character;
using Game.Runtime.ServiceLocator;

namespace Game.Runtime.Gameplay.Interactives
{
    public class WaitingForFoodTableInteraction : IInteraction
    {
        public string DebugName => nameof(WaitingForFoodTableInteraction);

        public void ExecuteInteraction(InteractableObject interactable)
        {
            var tableData = ServiceLocator<TableService>.GetService().GetTable(interactable.Id);
            var characterService = ServiceLocator<CharacterService>.GetService();
            
            tableData.Client.View.ShowHint(true, tableData.Client.OrderData.FoodId);

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