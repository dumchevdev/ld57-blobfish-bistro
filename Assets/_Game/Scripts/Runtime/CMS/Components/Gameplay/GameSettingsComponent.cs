using System;
using Game.Runtime._Game.Scripts.Runtime.Utils.Structs;
using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.CMS.Components.Gameplay
{
    [Serializable]
    public class GameSettingsComponent : CMSComponent
    {
        [Header("General")]
        public float SessionTimer;
        public float GoldRequired;
        
        [Header("Customers")]
        public RandomRangeFloat CustomersSpawnInterval;
        public int CustomerPatienceTimer;
        public float CustomerEatingTime;
        public float CustomerBrowsingMenuTime;

        [Header("Mods")] 
        public float GoalMod;
        public float OrderMod;
    }
}