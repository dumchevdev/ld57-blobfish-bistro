namespace Game.Runtime.Gameplay.Interactives
{
    public class QueueClientData
    {
        public ClientData ClientData;
        public int PointIndex;

        public QueueClientData(ClientData clientData, int pointIndex)
        {
            ClientData = clientData;
            PointIndex = pointIndex;
        }
    }
}