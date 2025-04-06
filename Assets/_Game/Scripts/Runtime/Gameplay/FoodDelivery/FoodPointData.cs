using UnityEngine;

namespace Game.Runtime.Gameplay.FoodDelivery
{
    public class FoodPointData
    {
        public Transform Point;
        public bool IsOccupied;

        public FoodPointData(Transform point)
        {
            Point = point;
        }
    }
}