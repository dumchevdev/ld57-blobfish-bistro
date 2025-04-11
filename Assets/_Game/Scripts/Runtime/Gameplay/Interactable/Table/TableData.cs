namespace Game.Runtime.Gameplay.Interactives
{
    public class TableData
    {
        public readonly int Id;
        public readonly TableBehaviour Behaviour;

        public TableData(int id, TableBehaviour behaviour)
        {
            Id = id;
            Behaviour = behaviour;
        }
    }
}