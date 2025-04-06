using UnityEngine;

namespace Game.Runtime.Gameplay.Interactives
{
    public class ClientBehaviour : InteractableObject
    {
        [SerializeField] private Animator animator;

        public void SetAnimator(AnimatorOverrideController clientAnimator)
        {
            animator.runtimeAnimatorController = clientAnimator;
        }

        public void SetMood(ClientMood mood)
        {
            // var clientMood = CMSProvider.GetEntity(CMSPrefabs.Gameplay.Client).GetComponent<ClientMoodComponent>();
            // _spriteRenderer.color = clientMood.ClientMoodViews.First(view => view.Mood == mood).Color;
        }

        public void SetSelectedState(bool selected)
        {

        }
    }
}