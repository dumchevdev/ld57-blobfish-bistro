using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.HUD
{
    [Serializable]
    public class StatisticPanel
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text title;
        
        public string ViewId { get; private set; }

        public void SetView(string viewId, Sprite sprite)
        {
            ViewId = viewId;
            icon.sprite = sprite;
        }

        public void UpdateCount(int count)
        {
            icon.transform.DOScale(2f, 0.1f).SetLoops(2, LoopType.Yoyo);
            title.text = count.ToString();
        }
    }
}