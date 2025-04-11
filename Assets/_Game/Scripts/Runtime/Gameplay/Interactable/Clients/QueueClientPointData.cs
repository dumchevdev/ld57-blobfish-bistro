using UnityEngine;

namespace Game.Runtime.Gameplay.Interactives
{
    public class QueueClientPointData
    {
        public readonly Transform Point;
        public bool IsOccupied;

        public QueueClientPointData(Transform point)
        {
            Point = point;
        }
    }
}