using System.Linq;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Gameplay;
using Game.Runtime.ServiceLocator;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Runtime.Gameplay.Interactives
{
    public class ClientBehaviour : InteractableObject
    {
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [SerializeField] private GameObject hint;
        [SerializeField] private SpriteRenderer iconSpriteRenderer;
        [SerializeField] private Sprite hintWarning;
        
        private bool isSelected;
        private static readonly int OutlineWidth = Shader.PropertyToID("_OutlineWidth");
        private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");

        private void Start()
        {
            spriteRenderer.material.SetFloat(OutlineWidth, 0f);
            ShowHint(false);
        }

        public void ShowHintWarning(bool isShow)
        {
            hint.SetActive(isShow);
            if (isShow)
            {
                iconSpriteRenderer.sprite = hintWarning;
            }
        }

        public void ShowHint(bool isShow, string foodId = null)
        {
            hint.SetActive(isShow);
            if (isShow && foodId != null)
            {
                var foods = CMSProvider.GetEntity(CMSPrefabs.Gameplay.Foods).GetComponent<FoodsComponent>();
                iconSpriteRenderer.sprite = foods.Foods.First(x => x.Id == foodId).Sprite;
            }
        }

        public void SetAnimator(AnimatorOverrideController clientAnimator)
        {
            animator.runtimeAnimatorController = clientAnimator;
        }

        public void SetMood(ClientMood mood)
        {
            // var clientMood = CMSProvider.GetEntity(CMSPrefabs.Gameplay.Client).GetComponent<ClientMoodComponent>();
            // _spriteRenderer.color = clientMood.ClientMoodViews.First(view => view.Mood == mood).Color;
        }

        private bool _blockSecondSelected;
        public void SetSelectedState(bool selected)
        {
            var service = ServiceLocator<ClientsService>.GetService();
            if (service.IsFirstInQueue(Id) && service.IsInQueue(Id))
            {
                isSelected = selected;
                spriteRenderer.material.SetFloat(OutlineWidth, selected ? 0.06f : 0);
                spriteRenderer.material.SetColor(OutlineColor, selected ? Color.green : Color.white);

                if (selected)
                {
                    _blockSecondSelected = true;
                    ServiceLocator<TableService>.GetService().ShowAllTable(Color.green, 0.06f, true);
                }
            }
            else if (_blockSecondSelected)
            {
                _blockSecondSelected = false;
                SetOutline();
                ServiceLocator<TableService>.GetService().ShowAllTable(Color.white, 0, false);
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            var service = ServiceLocator<ClientsService>.GetService();
            if (service.IsInQueue(Id))
            {
                if (!service.IsFirstInQueue(Id)) return;
                
                spriteRenderer.material.SetColor(OutlineColor, Color.white);
                spriteRenderer.material.SetFloat(OutlineWidth, 0.06f);
            }
            else
            {
                spriteRenderer.material.SetColor(OutlineColor, Color.white);
                spriteRenderer.material.SetFloat(OutlineWidth, 0.06f);
            }
           
        }
        
        public override void OnPointerExit(PointerEventData eventData)
        {
            
            var service = ServiceLocator<ClientsService>.GetService();
            if (service.IsInQueue(Id))
            {
                if (!service.IsFirstInQueue(Id)) return;
                
                spriteRenderer.material.SetColor(OutlineColor, Color.white);
                spriteRenderer.material.SetFloat(OutlineWidth, 0f);
            }
            else
            {
                spriteRenderer.material.SetColor(OutlineColor, Color.white);
                spriteRenderer.material.SetFloat(OutlineWidth, 0f);
            }
        }

        public void SetOutline()
        {
            spriteRenderer.material.SetColor(OutlineColor, Color.white);
            spriteRenderer.material.SetFloat(OutlineWidth, 0f);
            ShowHint(false);
            ShowHintWarning(false);
        }
    }
}