using System.Linq;
using Game.Runtime._Game.Scripts.Runtime.CMS;
using Game.Runtime._Game.Scripts.Runtime.CMS.Components.Gameplay;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.HUD;
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
        [SerializeField] private SpriteProgressBar progressBar;
        
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
            if (progressBar == null) return;
            var clampedProgress = Mathf.Clamp01(progress);
            var progressColor = clampedProgress >= 0.75f ? Color.green : clampedProgress >= 0.25f ? Color.yellow : Color.red;
            progressBar.SetProgress(progress, progressColor);
        }

        public void SetActiveProgressBar(bool isActive)
        {
            progressBar.SetActiveProgressBar(isActive);
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