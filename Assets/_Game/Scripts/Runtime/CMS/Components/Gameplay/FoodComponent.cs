using System;
using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.CMS.Components.Gameplay
{
    [Serializable]
    public class FoodComponent
    {
        public string Id;
        public int CookingTime;
        public Sprite Sprite;
        public float Price;
    }
}