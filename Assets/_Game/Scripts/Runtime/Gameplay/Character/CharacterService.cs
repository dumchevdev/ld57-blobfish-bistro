using System;
using System.Threading;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Commons;
using Game.Runtime.CMS.Components.Gameplay;
using Game.Runtime.Gameplay.FoodDelivery;
using Game.Runtime.Gameplay.Interactives;
using Game.Runtime.ServiceLocator;
using Game.Runtime.Utils.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Runtime.Gameplay.Character
{
    public class CharacterService : IService, IDisposable
    {
        public CharacterData CharacterData { get; private set; }
        public MovableBehaviour Movable { get; private set; }

        public void SpawnCharacter(Vector3 position)
        {
            var characterPrefab = CMSProvider.GetEntity(CMSPrefabs.Gameplay.Character).GetComponent<PrefabComponent>().Prefab;
            Movable = Object.Instantiate(characterPrefab, position, Quaternion.identity).GetComponent<MovableBehaviour>();
            CharacterData = new CharacterData();
        }

        public bool TryGetHandWithFood(string foodId, out CharacterHandData handData)
        {
            handData = null;
            
            if (CharacterData.LeftHand.FoodData != null && 
                CharacterData.LeftHand.FoodData.Id == foodId)
            {
                handData = CharacterData.LeftHand;
                return true;
            }

            if (CharacterData.RightHand.FoodData != null && 
                CharacterData.RightHand.FoodData.Id == foodId)
            {
                handData = CharacterData.RightHand;
                return true;
            }

            return false;
        }

        public CharacterHandData GetFreeHand()
        {
            if (CharacterData.LeftHand.FoodData == null)
                return CharacterData.LeftHand;
            
            if (CharacterData.RightHand.FoodData == null)
                return CharacterData.RightHand;

            return null;
        }

        public void Dispose()
        {
            Movable.Dispose();
        }
    }
}