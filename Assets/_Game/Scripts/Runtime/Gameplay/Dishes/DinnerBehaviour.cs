using Game.Runtime._Game.Scripts.Runtime.Gameplay.Interactable;
using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Dishes
{
    public class DinnerBehaviour : InteractableObject
    {
        public Transform Point;

        public void SetFoodSprite(Sprite sprite)
        {
            Settings.IsClickable = true;
            Settings.IsHighlightable = true;
            spriteRenderer.sprite = sprite;
        }
    }
}