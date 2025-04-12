using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Services.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay
{
    public class GameUiBehaviour : MonoBehaviour
    {
        [SerializeField] private GameObject disableUIInput;
        [SerializeField] private TMP_Text sessionTimer;
        [SerializeField] private Slider goalProgress;
        [SerializeField] private TMP_Text goalText;

        private void Start()
        {
            SetActivateUIInput(false);
        }

        public void SetActivateUIInput(bool activate)
        {
            disableUIInput.SetActive(activate);
        }

        public void InitializeGoalProgress(int goal)
        {
            goalText.text = $"0/{goal}"; 
            goalProgress.maxValue = goal;
            goalProgress.value = 0;
        }

        public void HandleGoalProgress(int currentValue, int goalValue)
        {
            goalText.text = $"{currentValue}/{goalValue}";
            goalProgress.value = Mathf.Clamp(currentValue, 0, goalProgress.maxValue);
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