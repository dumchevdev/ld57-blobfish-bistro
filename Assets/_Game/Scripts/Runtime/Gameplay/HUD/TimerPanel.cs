using System;
using DG.Tweening;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Services.Game;
using TMPro;
using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.HUD
{
    [Serializable]
    public class TimerPanel
    {
        [SerializeField] private TMP_Text sessionTimer;
        
        public void HandleSessionFinish()
        {
            sessionTimer.color = Color.red;
            sessionTimer.text = "17:00";
            sessionTimer.transform.DOScale(1.5f, 0.1f).SetLoops(2, LoopType.Yoyo);

        }
        
        public void UpdateTimeDisplay()
        {
            var sessionService = ServicesProvider.GetService<SessionTimerService>();
            if (sessionService.SessionFinished)
                return;

            float progress = Mathf.Min(sessionService.SessionProgress, 1.0f);
            int totalMinutes = (int)(progress * 8 * 60);
            int hours = 9 + totalMinutes / 60;
            int minutes = (totalMinutes % 60) / 10 * 10;

            sessionTimer.text = $"{hours:00}:{minutes:00}";
        }
    }
}