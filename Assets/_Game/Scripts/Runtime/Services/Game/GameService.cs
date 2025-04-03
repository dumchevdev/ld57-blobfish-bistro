using Game.Runtime.ServiceLocator;

namespace Game.Runtime.Framework.Services.Game
{
    public class GameService : IService
    {
        public bool IsPaused;
        
        private GameData _gameData;

        public GameService()
        {
            _gameData = new GameData();
        }
    }
}