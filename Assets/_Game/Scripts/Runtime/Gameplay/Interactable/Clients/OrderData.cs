using Game.Runtime.Gameplay.FoodDelivery;

namespace Game.Runtime.Gameplay.Interactives
{
    public class OrderData
    {
        public string FoodId;
        public FoodBehaviour FoodBehaviour;
        public bool IsClosed;
        
        public readonly ClientData ClientData;
        public readonly TableData TableData;

        public OrderData(ClientData clientData, TableData tableData)
        {
            ClientData = clientData;
            TableData = tableData;
        }
    }
}