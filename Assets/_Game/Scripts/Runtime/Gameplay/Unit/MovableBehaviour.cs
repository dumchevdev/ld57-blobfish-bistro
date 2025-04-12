using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Pathfinder;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Unit
{
    public class MovableBehaviour : MonoBehaviour, IDisposable
    {
        [SerializeField] private float _moveSpeed = 5f;
        
        private CancellationTokenSource _moveTokenSource;
        private bool _isMoving;

        public async UniTask MoveToPoint(Vector2 targetPosition, CancellationToken externalToken = default)
        {
            if (_isMoving) return;
            
            _moveTokenSource?.Cancel();
            _moveTokenSource = new CancellationTokenSource();
            
            var pathfinderService = ServiceLocator<PathfinderService>.GetService();
            var path = pathfinderService.FindPath(transform.position, targetPosition);
            
            if (path == null || path.Count == 0)
            {
                Debug.LogWarning("[MOVABLE] No path found");
                return;
            }
            
            var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(_moveTokenSource.Token, externalToken);

            try
            {
                _isMoving = true;

                foreach (var targetPoint in path)
                {
                    while (_moveTokenSource != null && !linkedToken.IsCancellationRequested)
                    {
                        if (this == null)
                            return;
                        
                        Vector2 direction = (targetPoint - (Vector2)transform.position).normalized;
                        transform.position += (Vector3)direction * (_moveSpeed * Time.deltaTime);
                        
                        if (Vector2.Distance(transform.position, targetPoint) <= 0.1f)
                            break;
                        
                        await UniTask.Yield(PlayerLoopTiming.Update, _moveTokenSource.Token, cancelImmediately: true);
                    }

                    if (_moveTokenSource != null && !linkedToken.IsCancellationRequested)
                    {
                        transform.position = targetPoint;
                    }
                }
            }
            finally
            {
                _isMoving = false;
                ResetMoveToken();
            }
        }

        private void ResetMoveToken()
        {
            _moveTokenSource?.Dispose();
            _moveTokenSource = null;
        }

        public void Dispose()
        {
            ResetMoveToken();
        }
    }
}