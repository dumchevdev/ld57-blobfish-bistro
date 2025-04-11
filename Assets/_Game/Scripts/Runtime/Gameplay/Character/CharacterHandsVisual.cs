using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Character
{
    public class CharacterHandsVisual : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer leftHandSprite;
        [SerializeField] private SpriteRenderer rightHandSprite;

        private void Start()
        {
            ResetHands();
        }

        public void SetHandSprite(Sprite foodSprite, bool isRightHand)
        {
            var spriteRenderer = isRightHand ? rightHandSprite : leftHandSprite;
            spriteRenderer.sprite = foodSprite;
            spriteRenderer.gameObject.SetActive(true);
        }

        public void ResetHandSprite(bool isRightHand)
        {
            var spriteRenderer = isRightHand ? rightHandSprite : leftHandSprite;
            spriteRenderer.gameObject.SetActive(false);
        }

        public void ResetHands()
        {
            leftHandSprite.gameObject.SetActive(false);
            rightHandSprite.gameObject.SetActive(false);
        }
    }
}