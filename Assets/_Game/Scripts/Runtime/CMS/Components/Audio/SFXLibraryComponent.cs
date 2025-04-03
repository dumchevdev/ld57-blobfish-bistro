using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime.CMS.Audio
{
    [Serializable]
    public class SFXLibraryComponent : CMSComponent
    {
        public List<AudioClip> Clips = new();
        public float Volume = 1f;
    }
}