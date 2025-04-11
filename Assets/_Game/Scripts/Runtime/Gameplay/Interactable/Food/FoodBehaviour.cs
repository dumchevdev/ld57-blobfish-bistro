using Game.Runtime.Gameplay.Interactives;
using UnityEngine;

namespace Game.Runtime.Gameplay.FoodDelivery
{
    public class FoodBehaviour : InteractableObject
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