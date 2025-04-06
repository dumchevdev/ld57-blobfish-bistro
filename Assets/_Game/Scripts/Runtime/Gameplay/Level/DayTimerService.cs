using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Gameplay;
using Game.Runtime.ServiceLocator;
using UnityEngine;

namespace Game.Runtime.Gameplay.Level
{
    public class DayTimerService : IService,IDisposable
    {
        public float CurrentTime;
        public bool DayFinished;

        private readonly float _dayTime;
        private CancellationTokenSource _dayTokenSource;

        public DayTimerService()
        {
            _dayTime = CMSProvider.GetEntity(CMSPrefabs.Gameplay.Day).GetComponent<DayComponent>().DayTimer;
        }

        public async UniTask StartDay()
        {
            Debug.Log($"[GAMEPLAY] Day timer started");

            _dayTokenSource?.Cancel();
            _dayTokenSource = new CancellationTokenSource();

            while (_dayTokenSource != null && !_dayTokenSource.IsCancellationRequested)
            {
                CurrentTime += Time.deltaTime;
                DayFinished = CurrentTime > _dayTime;
                
                await UniTask.Yield(cancellationToken: _dayTokenSource.Token);
            }
        }

        public void StopDay()
        {
            ResetDayToken();
        }

        private void ResetDayToken()
        {
            _dayTokenSource?.Dispose();
            _dayTokenSource = null;
        }

        public void Dispose()
        {
            ResetDayToken();
        }
    }
}