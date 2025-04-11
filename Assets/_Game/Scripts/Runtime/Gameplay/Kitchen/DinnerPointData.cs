using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Kitchen
{
    public class DinnerPointData
    {
        public bool IsOccupied;
        public readonly Transform Point;

        public DinnerPointData(Transform point)
        {
            Point = point;
        }
    }
}