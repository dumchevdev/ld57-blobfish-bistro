using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime._Game.Scripts.Runtime.CMS;
using Game.Runtime._Game.Scripts.Runtime.CMS.Components.Gameplay;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers.States;
using Game.Runtime.CMS;
using UnityEngine;

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
            _customerData.View.SetActiveProgressBar(true);

            _moodTokenSource?.Cancel();
            _moodTokenSource = new CancellationTokenSource();

            var timer = _patienceTimer;
            while (timer > 0)
            {
                if (_moodTokenSource == null || _moodTokenSource.IsCancellationRequested)
                    return;
                
                timer -= Time.deltaTime;
                _customerData.View.SetMoodProgress(timer/_patienceTimer);
                
                await UniTask.Yield(cancellationToken: _moodTokenSource.Token, cancelImmediately: true);
            }
            
            HandleMoodTimer();
        }
        
        public void ResetMoodTimer()
        {
            StartMoodTimer().Forget();
        }

        public void StopMoodTimer()
        {
            _moodTokenSource.Cancel();
            _customerData.View.SetActiveProgressBar(false);
        }

        private void HandleMoodTimer()
        {
            _customerData.StateMachine.ChangeState<LeavingClientState>();
            _customerData.View.SetMoodProgress(0);
            _customerData.View.SetActiveProgressBar(false);
        }

        public void Dispose()
        {
            _moodTokenSource?.Dispose();
            _moodTokenSource = null;
        }
    }
}