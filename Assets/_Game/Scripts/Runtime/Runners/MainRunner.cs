using Cysharp.Threading.Tasks;
using Game.Runtime.Framework.Services.Camera;
using Game.Runtime.Framework.Services.Game;
using Game.Runtime.Gameplay.Character;
using Game.Runtime.Gameplay.FoodDelivery;
using Game.Runtime.Gameplay.Interactives;
using Game.Runtime.Gameplay.Level;
using Game.Runtime.Gameplay.Pathfinder;
using Game.Runtime.ServiceLocator;
using Game.Runtime.Services.UI;
using UnityEngine;

namespace Game.Runtime.Runners
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
            ServiceLocator<FoodDeliveryService>.RegisterService(new FoodDeliveryService());
            ServiceLocator<DayTimerService>.RegisterService(new DayTimerService());
            ServiceLocator<PathfinderService>.RegisterService(new PathfinderService());
            ServiceLocator<CharacterService>.RegisterService(new CharacterService());
            ServiceLocator<TableService>.RegisterService(new TableService());
            ServiceLocator<ClientsService>.RegisterService(new ClientsService());
            ServiceLocator<LevelPointsService>.RegisterService(new LevelPointsService(spawnCharacterPoint, leavePoint, spawnClientPoint));
        }
        
        private void UnregisterServices()
        {
            ServiceLocator<GameService>.UnregisterService();
            ServiceLocator<CameraService>.UnregisterService();
            ServiceLocator<FoodDeliveryService>.UnregisterService();
            ServiceLocator<DayTimerService>.UnregisterService();
            ServiceLocator<PathfinderService>.UnregisterService();
            ServiceLocator<CharacterService>.UnregisterService();
            ServiceLocator<ClientsService>.UnregisterService();
            ServiceLocator<TableService>.UnregisterService();
            ServiceLocator<LevelPointsService>.UnregisterService();
        }

        private async UniTask StartGame()
        {
            ServiceLocator<PathfinderService>.GetService().CreateGrid();
            ServiceLocator<DayTimerService>.GetService().StartDay().Forget();
            ServiceLocator<CharacterService>.GetService().SpawnCharacter(spawnCharacterPoint.position);
            ServiceLocator<ClientsService>.GetService().StartQueueSpawning().Forget();
            
            await ServiceLocator<UIFaderService>.GetService().FadeOut();
        }

        private void OnDestroy()
        {
            UnregisterServices();
        }
    }
}