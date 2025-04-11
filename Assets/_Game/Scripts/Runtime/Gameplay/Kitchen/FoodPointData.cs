using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Kitchen
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