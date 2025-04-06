using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Runtime.Gameplay.Interactives
{
    public class ClientQueueChecker : IDisposable
    {
        private CancellationTokenSource _checkTokenSource;
        private readonly ClientsService _clientsService;

        public ClientQueueChecker(ClientsService clientsService)
        {
            _clientsService = clientsService;
            CheckClientQueue().Forget();
        }

        private async UniTask CheckClientQueue()
        {
            _checkTokenSource = new CancellationTokenSource();
            
            while (_checkTokenSource != null && !_checkTokenSource.IsCancellationRequested)
            {
                _clientsService.MoveQueueForward().Forget();
                await UniTask.WaitForSeconds(1f, cancellationToken: _checkTokenSource.Token);
            }
        }

        public void Dispose()
        {
            _checkTokenSource?.Dispose();
            _checkTokenSource = null;
        }
    }
}