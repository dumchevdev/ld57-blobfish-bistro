using System;
using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.CMS.Components.Gameplay
{
    [Serializable]
    public class DinnerComponent
    {
        public string Id;
        public int CookingTime;
        public Sprite Sprite;
        public int BasePrice;
    }
}