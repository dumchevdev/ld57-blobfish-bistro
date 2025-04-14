using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.HUD
{
    [Serializable]
    public class MoneyPanel
    {
        [SerializeField] private TMP_Text totalTitle;

        public void UpdateTitle(float money)
        {
            totalTitle.text = $"Total: <color=#00CF03>{money}$</color>";
            totalTitle.transform.DOScale(1.5f, 0.1f).SetLoops(2, LoopType.Yoyo);
        }
    }
}