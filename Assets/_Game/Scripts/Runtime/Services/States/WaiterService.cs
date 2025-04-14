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
        private CancellationTokenSource _mouseTokenSource;

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
        
        public async UniTask WaitClick()
        {
            _mouseTokenSource?.Cancel();
            _mouseTokenSource = new CancellationTokenSource();

            try
            {
                await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), cancellationToken: _mouseTokenSource.Token);
            }
            finally
            {
                ResetMouseToken();
            }
        }

        private void ResetWaitToken()
        {
            _waitTokenSource?.Dispose();
            _waitTokenSource = null;
        }

        private void ResetMouseToken()
        {
            _mouseTokenSource?.Dispose();
            _mouseTokenSource = null;
        }
        
        public void Dispose()
        {
            ResetWaitToken();
            ResetMouseToken();
        }
    }
}