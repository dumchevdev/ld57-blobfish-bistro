using System;
using Game.Runtime._Game.Scripts.Runtime.CMS;
using Game.Runtime._Game.Scripts.Runtime.CMS.Components.Commons;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Unit;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime.CMS;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Character
{
    public class CharacterService : IService, IDisposable
    {
        public CharacterData CharacterData { get; private set; }
        public MovableBehaviour Movable { get; private set; }
        public CharacterHandsVisual HandsVisual { get; private set; }

        public void SpawnCharacter(Vector3 position)
        {
            var characterPrefab = CMSProvider.GetEntity(CMSPrefabs.Gameplay.Character).GetComponent<PrefabComponent>().Prefab;
            var characterObject = Object.Instantiate(characterPrefab, position, Quaternion.identity);
            Movable = characterObject.GetComponent<MovableBehaviour>();
            HandsVisual = characterObject.GetComponent<CharacterHandsVisual>();
            CharacterData = new CharacterData();
        }

        public bool TryGetHandWithFood(string foodId, out CharacterHandData handData)
        {
            handData = null;
            
            if (CharacterData.LeftHand.DinnerData != null && 
                CharacterData.LeftHand.DinnerData.Id == foodId)
            {
                handData = CharacterData.LeftHand;
                return true;
            }

            if (CharacterData.RightHand.DinnerData != null && 
                CharacterData.RightHand.DinnerData.Id == foodId)
            {
                handData = CharacterData.RightHand;
                return true;
            }

            return false;
        }

        public CharacterHandData GetFreeHand()
        {
            if (CharacterData.LeftHand.DinnerData == null)
                return CharacterData.LeftHand;
            
            if (CharacterData.RightHand.DinnerData == null)
                return CharacterData.RightHand;

            return null;
        }

        public void Dispose()
        {
            Movable.Dispose();
        }
    }
}