using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Services.Save;
using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.HUD
{
    public class HUDBehaviour : MonoBehaviour
    {
        [SerializeField] private GameObject disableUIInput;
        [SerializeField] private TimerPanel timerPanel;
        [SerializeField] private GoalProgressBar goalProgressBar;
        [SerializeField] private StatisticsTable statisticTable;
        [SerializeField] private MoneyPanel moneyPanel;

        private void Start()
        {
            SetActivateUIInput(false);
            statisticTable.InitializeStatistics();
            UpdateMoneyPanel(ServicesProvider.GetService<SaveService>().SaveData.Statistics.Money);
        }

        public void SetActivateUIInput(bool activate)
        {
            disableUIInput.SetActive(activate);
        }

        public void UpdateMoneyPanel(float money)
        {
            moneyPanel.UpdateTitle(money);
        }

        public void InitializeGoalProgress(float goal)
        {
            goalProgressBar.InitializeGoalProgress(goal);
        }
        
        public void UpdateStatisticsPanel(string viewId)
        {
            statisticTable.UpdateStatisticsPanel(viewId);
        }

        public void HandleGoalProgress(float currentValue, float goalValue)
        {
            goalProgressBar.HandleGoalProgress(currentValue, goalValue);
        }

        public void HandleSessionFinish()
        {
            timerPanel.HandleSessionFinish();
        }

        private void Update()
        {
            timerPanel.UpdateTimeDisplay();
        }
    }
}