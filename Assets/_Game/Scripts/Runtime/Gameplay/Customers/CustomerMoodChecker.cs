using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime._Game.Scripts.Runtime.CMS;
using Game.Runtime._Game.Scripts.Runtime.CMS.Components.Gameplay;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers.States;
using Game.Runtime.CMS;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers
{
    public class CustomerMoodChecker : IDisposable
    {
        private readonly CustomerData _customerData;
        private readonly float _patienceTimer;
        private CancellationTokenSource _moodTokenSource;

        public CustomerMoodChecker(CustomerData customerData)
        {
            _customerData = customerData;
            _patienceTimer = CMSProvider.GetEntity(CMSPrefabs.Gameplay.GameSettings).GetComponent<GameSettingsComponent>().CustomerPatienceTimer;
        }
    
        private async UniTask StartMoodTimer()
        {
            _moodTokenSource?.Cancel();
            _moodTokenSource = new CancellationTokenSource();
            
            while (_moodTokenSource != null && !_moodTokenSource.IsCancellationRequested)
            {
                await UniTask.WaitForSeconds(_patienceTimer, cancellationToken: _moodTokenSource.Token);
                HandleMoodTimer();
            }
        }
        
        public void ResetMoodTimer()
        {
            StartMoodTimer().Forget();
        }

        public void StopMoodTimer()
        {
            _moodTokenSource.Cancel();
        }

        private void HandleMoodTimer()
        {
            _customerData.StateMachine.ChangeState<LeavingClientState>();
        }

        public void Dispose()
        {
            _moodTokenSource?.Dispose();
            _moodTokenSource = null;
        }
    }
}