using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Services.Audio;
using Game.Runtime.CMS;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Interactable
{
    public abstract class InteractableObject : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [HideInInspector] public int Id;
        public readonly InteractableSettings Settings = new();
        public IInteraction InteractionStrategy;

        [SerializeField] protected SpriteRenderer spriteRenderer;
        
        private readonly int _outlineColor = Shader.PropertyToID("_OutlineColor");
        private readonly int _outlineEnable = Shader.PropertyToID("_OutlineEnabled");

        protected void Start()
        {
            ResetBehaviour();
        }

        public void ShowOutline()
        {
            if (spriteRenderer == null) return;
            spriteRenderer.material.SetColor(_outlineColor, Settings.OutlineColor);
            spriteRenderer.material.SetInt(_outlineEnable, 1);
        }

        public void HideOutline()
        {
            if (spriteRenderer == null) return;
            spriteRenderer.material.SetInt(_outlineEnable, 0);
        }

        public void ResetBehaviour()
        {
            HideOutline();
            ResetBehaviorInternal();
        }
        
        protected virtual void ResetBehaviorInternal() { }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!Settings.IsClickable) return;
            
            InteractionStrategy?.ExecuteInteraction(this);
            if (InteractionStrategy != null)
            {
                ServiceLocator<AudioService>.GetService().Play(CMSPrefabs.Audio.SFX.SFXTyping);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!Settings.IsHighlightable) return;
            ShowOutline();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!Settings.IsHighlightable) return;
            HideOutline();
        }
    }
}