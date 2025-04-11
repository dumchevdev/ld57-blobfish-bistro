namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers
{
    public class CustomerInQueueData
    {
        public int PointIndex;
        public readonly CustomerData CustomerData;

        public CustomerInQueueData(CustomerData customerData, int pointIndex)
        {
            CustomerData = customerData;
            PointIndex = pointIndex;
        }
    }
}