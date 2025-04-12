using System;

namespace Game.Runtime._Game.Scripts.Runtime.Services.Game
{
    public class GameData
    {
        public event Action<int> OnGoldsChanged;
        
        public int Golds
        {
            get => _golds;
            set
            {
                OnGoldsChanged?.Invoke(value);
                _golds = value;
            }
        }

        private int _golds;
    }
}