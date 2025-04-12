using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Level
{
    public class LevelPointsService : IService
    {
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