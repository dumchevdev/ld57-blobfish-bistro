using System;
using Game.Runtime.Gameplay.Interactives;
using UnityEngine;

namespace Game.Runtime.CMS.Components.Gameplay
{
    public class ClientMoodComponent : CMSComponent
    {
        public ClientMoodView[] ClientMoodViews;
    }
    
    [Serializable]
    public class ClientMoodView
    {
        public ClientMood Mood;
        public Color Color;
    }
}