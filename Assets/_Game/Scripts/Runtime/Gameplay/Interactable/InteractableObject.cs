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
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // gameObject.GetComponent<SpriteRenderer>().color = Color.green;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}