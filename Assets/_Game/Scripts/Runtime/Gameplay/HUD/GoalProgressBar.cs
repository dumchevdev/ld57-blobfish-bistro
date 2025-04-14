using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.HUD
{
    [Serializable]
    public class GoalProgressBar
    {
        [SerializeField] private Slider goalProgress;
        [SerializeField] private TMP_Text goalText;
        
        public void InitializeGoalProgress(float goal)
        {
            goalText.text = $"0/{goal}$"; 
            goalProgress.maxValue = goal;
            goalProgress.value = 0;
        }
        
        public void HandleGoalProgress(float currentValue, float goalValue)
        {
            goalText.text = $"{currentValue}/{goalValue}$";
            goalProgress.DOValue(Mathf.Clamp(currentValue, 0, goalProgress.maxValue), 0.5f);
            goalText.transform.DOScale(1.5f, 0.1f).SetLoops(2, LoopType.Yoyo);
        }
    }
}