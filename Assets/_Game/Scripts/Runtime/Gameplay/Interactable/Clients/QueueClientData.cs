namespace Game.Runtime.Gameplay.Interactives
{
    public class QueueClientData
    {
        public int PointIndex;
        public readonly ClientData ClientData;

        public QueueClientData(ClientData clientData, int pointIndex)
        {
            ClientData = clientData;
            PointIndex = pointIndex;
        }
    }
}