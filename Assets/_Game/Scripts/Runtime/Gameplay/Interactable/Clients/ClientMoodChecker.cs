using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Runtime.Gameplay.Interactives
{
    public class ClientMoodChecker : IDisposable
    {
        private readonly ClientData _clientData;
        private CancellationTokenSource _moodTokenSource;

        public ClientMoodChecker(ClientData clientData)
        {
            _clientData = clientData;
        }
    
        public async UniTask StartMoodTimer()
        {
            _moodTokenSource = new CancellationTokenSource();
            
            while (_moodTokenSource != null && !_moodTokenSource.IsCancellationRequested)
            {
                await UniTask.WaitForSeconds(5f, cancellationToken: _moodTokenSource.Token);
                HandleMoodTimer();
            }
        }
        
        public void ResetMoodTimer()
        {
            _clientData.Mood = ClientMood.Happy;
            _moodTokenSource.Cancel();
            StartMoodTimer().Forget();
        }

        public void StopMoodTimer()
        {
            _moodTokenSource.Cancel();
        }

        private void HandleMoodTimer()
        {
            if (_clientData.Mood == ClientMood.Angry)
            {
                _clientData.StateMachine.ChangeState<LeavingClientState>();
                return;
            }

            _clientData.Mood++;
        }

        public void Dispose()
        {
            _moodTokenSource?.Dispose();
            _moodTokenSource = null;
        }
    }
}