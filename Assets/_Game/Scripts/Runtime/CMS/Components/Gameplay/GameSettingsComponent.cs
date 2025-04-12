using System;
using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.CMS.Components.Gameplay
{
    [Serializable]
    public class GameSettingsComponent : CMSComponent
    {
        [Header("General")]
        public float SessionTimer;
        public int GoldRequired;
        
        [Header("Customers")]
        public int CustomersSpawnInterval;
        public int CustomerPatienceTimer;
        public float CustomerEatingTime;
        public float CustomerBrowsingMenuTime;
    }
}