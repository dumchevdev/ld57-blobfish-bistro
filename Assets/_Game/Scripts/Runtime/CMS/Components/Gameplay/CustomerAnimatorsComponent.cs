using System;
using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.CMS.Components.Gameplay
{
    [Serializable]
    public class CustomerAnimatorsComponent : CMSComponent
    {
        public AnimatorOverrideController[] OverrideAnimators;
    }
}