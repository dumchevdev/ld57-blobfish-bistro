using Game.Runtime.Gameplay.Interactives;
using UnityEngine;

namespace Game.Runtime.Gameplay.FoodDelivery
{
    public class FoodBehaviour : InteractableObject
    {
        public Transform Point;

        [SerializeField] private SpriteRenderer spriteRenderer;
        
        public void SetFoodSprite(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
        }
    }
}