using Cysharp.Threading.Tasks;
using Game.Runtime.Framework.Services.Camera;
using Game.Runtime.Framework.Services.Game;
using Game.Runtime.ServiceLocator;
using Game.Runtime.Services.UI;
using UnityEngine;

namespace Game.Runtime.Runners
{
    public class MainRunner : MonoBehaviour
    {
        [SerializeField] private Camera _gameCamera;
        
        private void Start()
        {
            RegisterServices();
            StartGame().Forget();
        }

        private void RegisterServices()
        {
            ServiceLocator<GameService>.RegisterService(new GameService());
            ServiceLocator<CameraService>.RegisterService(new CameraService(_gameCamera));
        }
        
        private void UnregisterServices()
        {
            ServiceLocator<GameService>.UnregisterService();
            ServiceLocator<CameraService>.UnregisterService();
        }

        private async UniTask StartGame()
        {
            var sayService = ServiceLocator<UISayService>.GetService();
            var faderService = ServiceLocator<UIFaderService>.GetService();
            var cameraService = ServiceLocator<CameraService>.GetService();

            await faderService.FadeOut();
            cameraService.Shake(3, 0.5f);
            await sayService.Print("Wait for the jam to start...");
        }

        private void OnDestroy()
        {
            UnregisterServices();
        }
    }
}