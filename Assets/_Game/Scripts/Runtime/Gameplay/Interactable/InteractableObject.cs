using Game.Runtime.CMS;
using Game.Runtime.Framework.Services.Camera;
using Game.Runtime.ServiceLocator;
using Game.Runtime.Services.Audio;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Runtime.Gameplay.Interactives
{
    public abstract class InteractableObject : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public int Id;
        public readonly InteractableSettings Settings = new();
        public IInteraction InteractionStrategy;

        [SerializeField] protected SpriteRenderer spriteRenderer;
        
        private readonly int _outlineWidth = Shader.PropertyToID("_OutlineWidth");
        private readonly int _outlineColor = Shader.PropertyToID("_OutlineColor");

        protected void Start()
        {
            ResetBehaviour();
        }

        public void ShowOutline()
        {
            spriteRenderer.material.SetColor(_outlineColor, Settings.OutlineColor);
            spriteRenderer.material.SetFloat(_outlineWidth, 0.06f);
        }

        public void HideOutline()
        {
            spriteRenderer.material.SetFloat(_outlineWidth, 0);
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
                Debug.Log($"{InteractionStrategy.DebugName}");
                ServiceLocator<CameraService>.GetService().UIShake();
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