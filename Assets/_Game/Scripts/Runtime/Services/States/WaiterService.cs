using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime.ServiceLocator;
using Game.Runtime.Services.Input;
using UnityEngine;

namespace Game.Runtime.Services.States
{
    public class WaiterService : IService, IDisposable
    {
        private CancellationTokenSource _waitTokenSource;

        public async UniTask SmartWait(float f)
        {
            _waitTokenSource?.Cancel();
            _waitTokenSource = new CancellationTokenSource();

            try
            {
                while (f > 0 && !ServiceLocator<InputService>.GetService().OnMouseClickedAfFrame)
                {
                    f -= Time.deltaTime;
                    await UniTask.WaitForEndOfFrame(cancellationToken: _waitTokenSource.Token);
                }
            }
            finally
            {
                ResetWaitToken();
            }
        }
        
        public async UniTask WaitMouseClick()
        {
            _waitTokenSource?.Cancel();
            _waitTokenSource = new CancellationTokenSource();

            try
            {
                while (!ServiceLocator<InputService>.GetService().OnMouseClickedAfFrame)
                {
                    await UniTask.WaitForEndOfFrame(cancellationToken: _waitTokenSource.Token);
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