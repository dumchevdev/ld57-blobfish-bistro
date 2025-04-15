using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.HUD
{
    public class SpriteProgressBar : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer fillSprite;
        
        public void SetProgress(float progress, Color color)
        {
            var clampedProgress = Mathf.Clamp01(progress);
            var size = fillSprite.size;
            size.x = clampedProgress;
            fillSprite.size = size;
            fillSprite.color = color;
        }
        
        public void SetActiveProgressBar(bool isActive)
        {
            if (gameObject == null) return;
            gameObject.SetActive(isActive);
        }
    }
}