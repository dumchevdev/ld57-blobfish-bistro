namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Dishes
{
    public class DinnerData
    {
        public readonly string Id;
        public readonly DinnerBehaviour Behaviour;

        public DinnerData(string id, DinnerBehaviour behaviour)
        {
            Id = id;
            Behaviour = behaviour;
        }
    }
}