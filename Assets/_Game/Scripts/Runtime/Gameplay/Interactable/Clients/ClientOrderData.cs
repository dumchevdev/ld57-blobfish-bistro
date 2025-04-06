namespace Game.Runtime.Gameplay.Interactives
{
    public class ClientOrderData
    {
        public int TableId;
        public string FoodId;

        public ClientOrderData(string foodId, int tableId)
        {
            FoodId = foodId;
            TableId = tableId;
        }
    }
}