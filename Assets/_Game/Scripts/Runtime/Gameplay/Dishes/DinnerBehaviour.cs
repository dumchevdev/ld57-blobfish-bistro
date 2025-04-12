using Game.Runtime._Game.Scripts.Runtime.Gameplay.Interactable;
using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Dishes
{
    public class DinnerBehaviour : InteractableObject
    {
        public Transform Point;
        
        [SerializeField] private CircleCollider2D circleCollider;

        public void SetFoodSprite(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
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
    }
}