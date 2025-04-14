using Game.Runtime._Game.Scripts.Runtime.CMS;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Services.Audio;
using Game.Runtime._Game.Scripts.Runtime.Services.Camera;
using Game.Runtime._Game.Scripts.Runtime.Services.Save;
using Game.Runtime._Game.Scripts.Runtime.Services.States;
using Game.Runtime._Game.Scripts.Runtime.Services.Statistics;
using Game.Runtime._Game.Scripts.Runtime.Services.UI;
using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Runners
{
    public class GlobalRunner : MonoBehaviour
    {
        private static bool _isRunning;
        private static readonly ServiceScope _globalScope = ServiceScope.Global;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InstantiateAutoSaveSystem()
        {
            if (!_isRunning)
            {
                GameObject servicedMain = new GameObject(nameof(GlobalRunner));
                servicedMain.AddComponent<GlobalRunner>();
                
                DontDestroyOnLoad(servicedMain);
                _isRunning = true;
            }
        }
        
        private void Awake()
        {
            Debug.Log("[GlobalRunner] Entry point!");
            CMSProvider.Reload();
            RegisterServices();
            ServicesProvider.InitializeServices(_globalScope);
        }

        private void RegisterServices()
        {
            ServicesProvider.RegisterService<AudioService>(new AudioService(), _globalScope);
            ServicesProvider.RegisterService<SaveService>(new SaveService(), _globalScope);
            ServicesProvider.RegisterService<WaiterService>(new WaiterService(), _globalScope);
            ServicesProvider.RegisterService<UITextService>(new UITextService(), _globalScope);
            ServicesProvider.RegisterService<UIService>(new UIService(), _globalScope);
            ServicesProvider.RegisterService<StatisticsService>(new StatisticsService(), _globalScope);
            ServicesProvider.RegisterService<CameraService>(new CameraService(), _globalScope);
        }

        private void OnDestroy()
        {
            ServicesProvider.Clear();
            _isRunning = false;
        }
    }
}