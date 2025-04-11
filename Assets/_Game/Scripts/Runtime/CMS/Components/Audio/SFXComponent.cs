using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.CMS.Components.Audio
{
    [Serializable]
    public class SFXComponent : CMSComponent
    {
        public List<AudioClip> Clips = new();
        public float Volume = 1f;
        public float Cooldown = 0.1f;
    }
}