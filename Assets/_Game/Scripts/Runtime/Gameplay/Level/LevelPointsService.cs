using Game.Runtime.ServiceLocator;
using UnityEngine;

namespace Game.Runtime.Gameplay.Level
{
    public class LevelPointsService : IService
    {
        public Transform SpawnCharacterPoint {get; private set;}
        public Transform SpawnClientPoint {get; private set;}
        public Transform SpawnPoint {get; private set;}
        public Transform LeavePoint {get; private set;}
        
        public LevelPointsService(Transform spawnPoint, Transform leavePoint, Transform spawnClientPoint)
        {
            SpawnPoint = spawnPoint;
            LeavePoint = leavePoint;
            SpawnClientPoint = spawnClientPoint;
        }
    }
}