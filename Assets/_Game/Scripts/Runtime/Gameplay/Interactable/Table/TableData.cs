using Game.Runtime.Gameplay.FoodDelivery;

namespace Game.Runtime.Gameplay.Interactives
{
    public class TableData
    {
        public int Id;
        public ClientData Client;
        public FoodData Food;
        public TableBehaviour Behaviour;

        public TableData(int id, TableBehaviour behaviour)
        {
            Id = id;
            Behaviour = behaviour;
        }
    }
}