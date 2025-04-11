using System.Linq;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Gameplay;
using UnityEngine;

namespace Game.Runtime.Gameplay.Interactives
{
    public class ClientBehaviour : InteractableObject
    {
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject hint;
        [SerializeField] private SpriteRenderer iconSpriteRenderer;
        [SerializeField] private Sprite hintWarning;

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
                var foods = CMSProvider.GetEntity(CMSPrefabs.Gameplay.Foods).GetComponent<FoodsComponent>();
                iconSpriteRenderer.sprite = foods.Foods.First(x => x.Id == foodId).Sprite;
            }
        }

        public void SetAnimator(AnimatorOverrideController clientAnimator)
        {
            animator.runtimeAnimatorController = clientAnimator;
        }

        public void SetMood(ClientMood mood)
        {
            // var clientMood = CMSProvider.GetEntity(CMSPrefabs.Gameplay.Client).GetComponent<ClientMoodComponent>();
            // _spriteRenderer.color = clientMood.ClientMoodViews.First(view => view.Mood == mood).Color;
        }


        protected override void ResetBehaviorInternal()
        {
            ShowHint(false);
            ShowHintWarning(false);
        }
    }
}