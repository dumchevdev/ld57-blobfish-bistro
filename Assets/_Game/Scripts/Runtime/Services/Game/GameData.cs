using System;

namespace Game.Runtime._Game.Scripts.Runtime.Services.Game
{
    public class GameData
    {
        public event Action<float> OnGoldsChanged;
        
        public float Money
        {
            get => _money;
            set
            {
                OnGoldsChanged?.Invoke(value);
                _money = value;
            }
        }
        
        private float _money;
        
    }
}