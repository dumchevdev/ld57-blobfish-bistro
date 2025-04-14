using Cysharp.Threading.Tasks;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Character;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.HUD;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Kitchen;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Level;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Pathfinder;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Services.Camera;
using Game.Runtime._Game.Scripts.Runtime.Services.Game;
using Game.Runtime._Game.Scripts.Runtime.Services.Save;
using Game.Runtime._Game.Scripts.Runtime.Services.UI;
using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Runners
{
    public class GameRunner : MonoBehaviour
    {
        [SerializeField] private Camera gameCamera;
        [SerializeField] private Transform spawnCharacterPoint;
        [SerializeField] private Transform spawnClientPoint;
        [SerializeField] private Transform leavePoint;

        private readonly ServiceScope _gameScope = ServiceScope.Game;
        
        private void Start()
        {
            RegisterCamera();
            RegisterServices();
            StartGame().Forget();
        }

        private void RegisterCamera()
        {
            ServicesProvider.GetService<CameraService>().RegisterCamera(gameCamera);
        }

        private void RegisterServices()
        {
            ServicesProvider.RegisterService<GameService>(new GameService(), _gameScope);
            ServicesProvider.RegisterService<KitchenService>(new KitchenService(), _gameScope);
            ServicesProvider.RegisterService<SessionTimerService>(new SessionTimerService(), _gameScope);
            ServicesProvider.RegisterService<PathfinderService>(new PathfinderService(), _gameScope);
            ServicesProvider.RegisterService<CharacterService>(new CharacterService(), _gameScope);
            ServicesProvider.RegisterService<LevelPointsService>(new LevelPointsService(spawnCharacterPoint, leavePoint, spawnClientPoint), ServiceScope.Game);
            ServicesProvider.RegisterService<HUDService>(new HUDService(), _gameScope);
        }

        private async UniTask StartGame()
        {
            ServicesProvider.InitializeServices(_gameScope);
            
            ServicesProvider.GetService<HUDService>().SetActiveUIInput(true);
            
            ServicesProvider.GetService<PathfinderService>().CreateGrid();
            ServicesProvider.GetService<CharacterService>().SpawnCharacter(spawnCharacterPoint.position);
            
            await ServicesProvider.GetService<UIService>()
                .ShowLevelTitle(ServicesProvider.GetService<SaveService>().SaveData.Level);
            
            ServicesProvider.GetService<SessionTimerService>().StartTimer().Forget();
            ServicesProvider.GetService<GameService>().StartGameLoop();
            
            ServicesProvider.GetService<HUDService>().SetActiveUIInput(false);
        }


        private void OnDestroy()
        {
            ServicesProvider.DisposeScope(_gameScope);
        }
    }
}