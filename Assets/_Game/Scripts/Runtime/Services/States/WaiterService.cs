using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime.ServiceLocator;
using UnityEngine;

namespace Game.Runtime.Services.States
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
        
        public async UniTask WaitTimer(float time, CancellationTokenSource tokenSource, Action onTimeout = null)
        {
            try
            {
                while (tokenSource != null && !tokenSource.IsCancellationRequested && time > 0)
                {
                    time -= Time.deltaTime;
                    await UniTask.Yield(tokenSource.Token, cancelImmediately: true)
                        .SuppressCancellationThrow();
                }
            }
            finally
            {
                tokenSource?.Dispose();
                onTimeout?.Invoke();
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