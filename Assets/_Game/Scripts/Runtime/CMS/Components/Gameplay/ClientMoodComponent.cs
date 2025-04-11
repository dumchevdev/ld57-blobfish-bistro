using System;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Customers;
using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.CMS.Components.Gameplay
{
    public class ClientMoodComponent : CMSComponent
    {
        public ClientMoodView[] ClientMoodViews;
    }

    [Serializable]
    public class ClientMoodView
    {
        public CustomerMood Mood;
        public Color Color;
    }
}