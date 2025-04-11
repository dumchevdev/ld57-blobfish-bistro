using Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Dishes;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Tables;

namespace Game.Runtime._Game.Scripts.Runtime.Services.Game
{
    public class OrderData
    {
        public string FoodId;
        public DinnerBehaviour DinnerBehaviour;
        public bool IsClosed;
        public bool OrderAlreadyTaken;
        
        public readonly CustomerData CustomerData;
        public readonly TableData TableData;

        public OrderData(CustomerData customerData, TableData tableData)
        {
            CustomerData = customerData;
            TableData = tableData;
        }
    }
}