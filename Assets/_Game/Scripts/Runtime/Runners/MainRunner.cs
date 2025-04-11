using Cysharp.Threading.Tasks;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Character;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Kitchen;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Level;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.Pathfinder;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Services.Camera;
using Game.Runtime._Game.Scripts.Runtime.Services.Game;
using Game.Runtime._Game.Scripts.Runtime.Services.UI;
using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Runners
{
    public class MainRunner : MonoBehaviour
    {
        [SerializeField] private Camera gameCamera;
        [SerializeField] private Transform spawnCharacterPoint;
        [SerializeField] private Transform spawnClientPoint;
        [SerializeField] private Transform leavePoint;
        
        private void Start()
        {
            RegisterServices();
            StartGame().Forget();
        }

        private void RegisterServices()
        {
            ServiceLocator<GameService>.RegisterService(new GameService());
            ServiceLocator<CameraService>.RegisterService(new CameraService(gameCamera));
            ServiceLocator<KitchenService>.RegisterService(new KitchenService());
            ServiceLocator<DayTimerService>.RegisterService(new DayTimerService());
            ServiceLocator<PathfinderService>.RegisterService(new PathfinderService());
            ServiceLocator<CharacterService>.RegisterService(new CharacterService());
            ServiceLocator<LevelPointsService>.RegisterService(new LevelPointsService(spawnCharacterPoint, leavePoint, spawnClientPoint));
        }
        
        private void UnregisterServices()
        {
            ServiceLocator<GameService>.UnregisterService();
            ServiceLocator<CameraService>.UnregisterService();
            ServiceLocator<KitchenService>.UnregisterService();
            ServiceLocator<DayTimerService>.UnregisterService();
            ServiceLocator<PathfinderService>.UnregisterService();
            ServiceLocator<CharacterService>.UnregisterService();
            ServiceLocator<LevelPointsService>.UnregisterService();
        }

        private async UniTask StartGame()
        {
            ServiceLocator<PathfinderService>.GetService().CreateGrid();
            ServiceLocator<DayTimerService>.GetService().StartDay().Forget();
            ServiceLocator<CharacterService>.GetService().SpawnCharacter(spawnCharacterPoint.position);
            ServiceLocator<GameService>.GetService().StartGameLoop();
            
            await ServiceLocator<UIFaderService>.GetService().FadeOut();
        }

        private void OnDestroy()
        {
            UnregisterServices();
        }
    }
}