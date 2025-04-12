using Game.Runtime._Game.Scripts.Runtime.Gameplay.Interactable;
using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Tables
{
    public class TableBehaviour : InteractableObject
    { 
        public Transform CharacterPoint;
        public Transform ClientPoint;
        public Transform FoodPoint;
        public bool IsRight;
    }
}