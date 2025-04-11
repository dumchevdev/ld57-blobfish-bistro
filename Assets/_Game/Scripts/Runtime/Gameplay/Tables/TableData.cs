namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Tables
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