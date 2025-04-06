namespace Game.Runtime.Gameplay.FoodDelivery
{
    public class FoodData
    {
        public string Id;
        public FoodBehaviour Behaviour;
        public FoodPointData FoodPointData;

        public FoodData(string id, FoodBehaviour behaviour)
        {
            Id = id;
            Behaviour = behaviour;
        }
    }
}