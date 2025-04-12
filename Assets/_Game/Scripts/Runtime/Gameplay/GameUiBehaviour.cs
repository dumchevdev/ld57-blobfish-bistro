using DG.Tweening;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Services.Game;
using TMPro;
using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay
{
    public class GameUiBehaviour : MonoBehaviour
    {
        [SerializeField] private GameObject disableUIInput;
        [SerializeField] private TMP_Text sessionTimer;

        private void Start()
        {
            SetActivateUIInput(false);
        }

        public void SetActivateUIInput(bool activate)
        {
            disableUIInput.SetActive(activate);
        }

        public void HandleSessionFinish()
        {
            sessionTimer.color = Color.red;
            sessionTimer.text = "17:00";
        }

        private void Update()
        {
            UpdateTimeDisplay();
        }
        
        private void UpdateTimeDisplay()
        {
            var sessionService = ServiceLocator<SessionTimerService>.GetService();
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