using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime._Game.Scripts.Runtime.CMS;
using Game.Runtime._Game.Scripts.Runtime.CMS.Components.Gameplay;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime.CMS;
using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Services.Game
{
    public class SessionTimerService : IService, IDisposable
    {
        public event Action OnSessionTimerFinished;
        
        public float CurrentTime { get; private set; }
        public bool SessionFinished;

        public float SessionProgress => CurrentTime / _sessionTimer;

        private readonly float _sessionTimer;
        private CancellationTokenSource _timerTokenSource;

        public SessionTimerService()
        {
            _sessionTimer = CMSProvider.GetEntity(CMSPrefabs.Gameplay.GameSettings).GetComponent<GameSettingsComponent>().SessionTimer;
        }

        public async UniTask StartTimer()
        {
            if (SessionFinished) return;
            
            _timerTokenSource = new CancellationTokenSource();

            while (_timerTokenSource != null && !_timerTokenSource.IsCancellationRequested)
            {
                CurrentTime += Time.deltaTime;
                SessionFinished = CurrentTime > _sessionTimer;

                if (SessionFinished)
                {
                    OnSessionTimerFinished?.Invoke();
                    _timerTokenSource.Cancel();
                    return;
                }
                
                await UniTask.Yield(_timerTokenSource.Token, true);
            }
        }

        public void Dispose()
        {
            _timerTokenSource?.Dispose();
            _timerTokenSource = null;
        }
    }
}