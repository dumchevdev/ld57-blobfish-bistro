namespace Game.Runtime.Gameplay.FoodDelivery
{
    public class FoodData
    {
        public readonly string Id;
        public readonly FoodBehaviour Behaviour;

        public FoodData(string id, FoodBehaviour behaviour)
        {
            Id = id;
            Behaviour = behaviour;
        }
    }
}