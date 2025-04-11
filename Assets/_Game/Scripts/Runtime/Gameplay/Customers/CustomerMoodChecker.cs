using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers.States;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers
{
    public class CustomerMoodChecker : IDisposable
    {
        private readonly CustomerData _customerData;
        private CancellationTokenSource _moodTokenSource;

        public CustomerMoodChecker(CustomerData customerData)
        {
            _customerData = customerData;
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
            _customerData.Mood = CustomerMood.Happy;
            _moodTokenSource.Cancel();
            StartMoodTimer().Forget();
        }

        public void StopMoodTimer()
        {
            _moodTokenSource.Cancel();
        }

        private void HandleMoodTimer()
        {
            if (_customerData.Mood == CustomerMood.Angry)
            {
                _customerData.StateMachine.ChangeState<LeavingClientState>();
                return;
            }

            _customerData.Mood++;
        }

        public void Dispose()
        {
            _moodTokenSource?.Dispose();
            _moodTokenSource = null;
        }
    }
}