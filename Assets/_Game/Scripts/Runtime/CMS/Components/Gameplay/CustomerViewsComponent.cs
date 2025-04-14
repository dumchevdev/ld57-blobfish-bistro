using System;
using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.CMS.Components.Gameplay
{
    [Serializable]
    public class CustomerViewsComponent : CMSComponent
    {
        public CustomerView[] Views;
    }

    [Serializable]
    public class CustomerView
    {
        public string ViewId => $"{OverrideAnimator.name}.{Sprite.name}";
        
        public AnimatorOverrideController OverrideAnimator;
        public Sprite Sprite;
    }
}