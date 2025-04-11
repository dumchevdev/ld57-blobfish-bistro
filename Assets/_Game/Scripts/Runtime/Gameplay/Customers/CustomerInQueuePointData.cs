using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers
{
    public class CustomerInQueuePointData
    {
        public readonly Transform Point;
        public bool IsOccupied;

        public CustomerInQueuePointData(Transform point)
        {
            Point = point;
        }
    }
}