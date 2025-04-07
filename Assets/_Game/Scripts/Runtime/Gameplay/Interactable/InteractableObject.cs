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
        public int Id { get; private set; }
        
        public IInteraction _current;

        public void SetId(int id)
        {
            Id = id;
        }
    
        public void SetStrategy(IInteraction strategy)
        {
            _current = strategy;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            _current?.ExecuteInteraction(this);
            if (_current != null)
            {
                ServiceLocator<CameraService>.GetService().UIShake();
                ServiceLocator<AudioService>.GetService().Play(CMSPrefabs.Audio.SFX.SFXTyping);
            }
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
        }
    }
}