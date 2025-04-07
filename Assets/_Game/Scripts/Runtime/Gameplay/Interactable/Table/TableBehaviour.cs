using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Runtime.Gameplay.Interactives
{
    public class TableBehaviour : InteractableObject
    { 
        public Transform CharacterPoint;
        public Transform ClientPoint;
        public Transform FoodPoint;
        
        [SerializeField] private SpriteRenderer _renderer;

        private static readonly int OutlineWidth = Shader.PropertyToID("_OutlineWidth");
        private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");

        private bool blockedOutline;

        public void SetOutlineColor(Color color, float wight, bool blocked)
        {
            _renderer.material.SetColor(OutlineColor, color);
            _renderer.material.SetFloat(OutlineWidth, wight);
            blockedOutline = blocked;
        }
        
        public void Start()
        {
            _renderer.material.SetFloat(OutlineWidth, 0f);
        }
        
        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (blockedOutline) return;
            _renderer.material.SetColor(OutlineColor, Color.white);
            _renderer.material.SetFloat(OutlineWidth, 0.06f);
        }
        
        public override void OnPointerExit(PointerEventData eventData)
        {
            if (blockedOutline) return;
            _renderer.material.SetColor(OutlineColor, Color.white);
            _renderer.material.SetFloat(OutlineWidth, 0f);
        }
    }
}