using System.Collections.Generic;
using Game.Runtime.ServiceLocator;
using UnityEngine;

namespace Game.Runtime.Gameplay.Pathfinder
{
    public class PathfinderService : IService
    {
        private readonly PathfinderBehaviour _pathfinderBehaviour;

        public PathfinderService()
        {
            var pathfinderObject = new GameObject(nameof(PathfinderBehaviour));
            _pathfinderBehaviour = pathfinderObject.AddComponent<PathfinderBehaviour>();
        }

        public void CreateGrid()
        {
            Debug.Log($"[GAMEPLAY] Pathfinder grid created");
            _pathfinderBehaviour.CreateGrid();
        }
        
        public List<Vector2> FindPath(Vector2 startPos, Vector2 targetPos)
        {
            return _pathfinderBehaviour.FindPath(startPos, targetPos);
        }
    }
}