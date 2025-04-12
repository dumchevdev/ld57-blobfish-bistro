using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Services.States
{
    public class WaiterService : IService, IDisposable
    {
        private CancellationTokenSource _waitTokenSource;

        public async UniTask SmartWait(float time)
        {
            var skipped = false;

            _waitTokenSource?.Cancel();
            _waitTokenSource = new CancellationTokenSource();

            try
            {
                while (time > 0 && !skipped)
                {
                    if (_waitTokenSource == null || _waitTokenSource.IsCancellationRequested)
                        return;
                    
                    time -= Time.deltaTime;
                    skipped = Input.GetMouseButtonDown(0);
                    
                    await UniTask.Yield();
                }
            }
            finally
            {
                ResetWaitToken();
            }
        }

        private void ResetWaitToken()
        {
            _waitTokenSource?.Dispose();
            _waitTokenSource = null;
        }

        
        public void Dispose()
        {
            ResetWaitToken();
        }
    }
}