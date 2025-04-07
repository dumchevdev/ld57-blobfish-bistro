using System;
using Game.Runtime.Gameplay.Interactives;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Runtime.Gameplay.FoodDelivery
{
    public class FoodBehaviour : InteractableObject
    {
        public Transform Point;

        [SerializeField] private SpriteRenderer spriteRenderer;
        
        private static readonly int OutlineWidth = Shader.PropertyToID("_OutlineWidth");
        private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");
        
        public bool Interacting = true;

        public void Start()
        {
            spriteRenderer.material.SetFloat(OutlineWidth, 0f);
        }

        public void SetFoodSprite(Sprite sprite)
        {
            Interacting = true;
            spriteRenderer.sprite = sprite;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (!Interacting) return;
            spriteRenderer.material.SetColor(OutlineColor, Color.white);
            spriteRenderer.material.SetFloat(OutlineWidth, 0.06f);
        }
        
        public override void OnPointerExit(PointerEventData eventData)
        {
            if (!Interacting) return;
            spriteRenderer.material.SetColor(OutlineColor, Color.white);
            spriteRenderer.material.SetFloat(OutlineWidth, 0f);
        }
    }
}