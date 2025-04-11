using Game.Runtime._Game.Scripts.Runtime.Gameplay.Dishes;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.Character
{
    public class CharacterHandData
    {
        public bool IsRightHand { get;  }
        public DinnerData DinnerData;

        public CharacterHandData(bool isRightHand)
        {
            IsRightHand = isRightHand;
        }
    }
}