using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.HUD;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Interactable;
using Game.Runtime._Game.Scripts.Runtime.Utils.Helpers;
using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Dishes
{
    public class DinnerBehaviour : InteractableObject
    {
        public Transform Point;
        
        [SerializeField] private CircleCollider2D circleCollider;
        [SerializeField] private SpriteProgressBar spriteProgressBar;
        [SerializeField] private GameObject moneyIcon;

        private CancellationTokenSource _progressTokenSource;

        public void SetFoodSprite(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
            moneyIcon.SetActive(false);
        }

        public void SetMoneyState(Sprite emptySprite)
        {
            spriteRenderer.sprite = emptySprite;
            moneyIcon.SetActive(true);
        }

        public void SetQueueState()
        {
            spriteProgressBar.SetActiveProgressBar(false);
            Helpers.ColorHelper.SetAlpha(spriteRenderer, 0.5f);
            gameObject.SetActive(true);
        }

        public void StartCookingTimer(float timer)
        {
            spriteProgressBar.SetActiveProgressBar(true);
            StartTimer(timer).Forget();
        }
        
        private async UniTask StartTimer(float timer)
        {
            _progressTokenSource = new CancellationTokenSource();
            
            var timerInternal = timer;
            while (timerInternal > 0)
            {
                if (_progressTokenSource == null || _progressTokenSource.IsCancellationRequested)
                    return;
                
                var progress = 1 - timerInternal/timer;
                spriteProgressBar.SetProgress(progress, Color.green);
                timerInternal -= Time.deltaTime;

                await UniTask.Yield(cancellationToken: _progressTokenSource.Token, cancelImmediately: true);
            }
            
            spriteProgressBar.SetProgress(1, Color.green);
        }

        public void SetFishingState()
        {
            Helpers.ColorHelper.SetAlpha(spriteRenderer, 1);
            spriteProgressBar.SetActiveProgressBar(false);
            transform.DOScale(1.5f, 0.1f).SetLoops(2, LoopType.Yoyo);

            _progressTokenSource?.Dispose();
            _progressTokenSource = null;
        }

        public void EnableInteraction()
        {
            Settings.IsClickable = true;
            Settings.IsHighlightable = true;
            circleCollider.enabled = true;
        }

        protected override void ResetBehaviorInternal()
        {
            if (circleCollider == null) return;
            circleCollider.enabled = false;
        }

        private void OnDestroy()
        {
            _progressTokenSource?.Dispose();
            _progressTokenSource = null;
        }
    }
}