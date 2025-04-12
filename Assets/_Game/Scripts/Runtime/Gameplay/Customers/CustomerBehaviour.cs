using System.Linq;
using Game.Runtime._Game.Scripts.Runtime.CMS;
using Game.Runtime._Game.Scripts.Runtime.CMS.Components.Gameplay;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Interactable;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Unit;
using Game.Runtime.CMS;
using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers
{
    public class CustomerBehaviour : InteractableObject
    {
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject hint;
        [SerializeField] private SpriteRenderer iconSpriteRenderer;
        [SerializeField] private Sprite hintWarning;
        [SerializeField] private SpriteFlipper flipper;
        [SerializeField] private SpriteRenderer moodProgress;
        [SerializeField] private SpriteRenderer progressBar;

        public void ShowHintWarning(bool isShow)
        {
            hint.SetActive(isShow);
            if (isShow)
            {
                iconSpriteRenderer.sprite = hintWarning;
            }
        }

        public void ShowHint(bool isShow, string foodId = null)
        {
            hint.SetActive(isShow);
            if (isShow && foodId != null)
            {
                var foods = CMSProvider.GetEntity(CMSPrefabs.Gameplay.DishesLibrary).GetComponent<DishesLibraryComponent>();
                iconSpriteRenderer.sprite = foods.Dishes.First(x => x.Id == foodId).Sprite;
            }
        }

        public void SetMoodProgress(float progress)
        {
            if (moodProgress == null) return;
            var clampedProgress = Mathf.Clamp01(progress);
            var progressColor = clampedProgress >= 0.75f ? Color.green : clampedProgress >= 0.25f ? Color.yellow : Color.red;
            var size = moodProgress.size;
            size.x = clampedProgress;
            moodProgress.size = size;
            moodProgress.color = progressColor;
        }

        public void SetActiveProgressBar(bool isActive)
        {
            if (progressBar == null) return;
            progressBar.gameObject.SetActive(isActive);
        }

        public void ForceFlip(bool isRight)
        {
            flipper.ForceFlip(isRight);
        }

        public void SetBlockFlipper(bool isBlocked)
        {
            flipper.BlockFlipper = isBlocked;
        }
        
        public void SetAnimator(AnimatorOverrideController clientAnimator)
        {
            animator.runtimeAnimatorController = clientAnimator;
        }

        protected override void ResetBehaviorInternal()
        {
            ShowHint(false);
            ShowHintWarning(false);
        }
    }
}