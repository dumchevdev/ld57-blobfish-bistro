using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime.ServiceLocator;
using UnityEngine;

namespace Game.Runtime.Services.States
{
    public class WaiterService : IService, IDisposable
    {
        private bool _isSkipped;
        private CancellationTokenSource _waitTokenSource;

        public async UniTask SmartWait(float f)
        {
            _waitTokenSource?.Cancel();
            _waitTokenSource = new CancellationTokenSource();

            try
            {
                while (f > 0 && !_isSkipped)
                {
                    f -= Time.deltaTime;
                    _isSkipped = Input.GetMouseButtonDown(0);
                    await UniTask.Yield(cancellationToken: _waitTokenSource.Token);
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
                while (!_isSkipped)
                {
                    _isSkipped = Input.GetMouseButtonDown(0);
                    await UniTask.Yield(cancellationToken: _waitTokenSource.Token);
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