using Game.Runtime._Game.Scripts.Runtime.CMS;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Services.Audio;
using Game.Runtime._Game.Scripts.Runtime.Services.Save;
using Game.Runtime._Game.Scripts.Runtime.Services.States;
using Game.Runtime._Game.Scripts.Runtime.Services.Statistics;
using Game.Runtime._Game.Scripts.Runtime.Services.UI;
using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Runners
{
    public class GameRunner : MonoBehaviour
    {
        private static bool _isRunning;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InstantiateAutoSaveSystem()
        {
            if (!_isRunning)
            {
                GameObject servicedMain = new GameObject("GameRunner");
                servicedMain.AddComponent<GameRunner>();
                
                DontDestroyOnLoad(servicedMain);
                _isRunning = true;
            }
        }
        
        private void Awake()
        {
            CMSProvider.Load();
            RegisterServices();
        }

        private void RegisterServices()
        {
            ServiceLocator<AudioService>.RegisterService(new AudioService());
            ServiceLocator<SaveService>.RegisterService(new SaveService());
            ServiceLocator<WaiterService>.RegisterService(new WaiterService());
            ServiceLocator<UITextService>.RegisterService(new UITextService());
            ServiceLocator<UISayService>.RegisterService(new UISayService());
            ServiceLocator<UIFaderService>.RegisterService(new UIFaderService());
            ServiceLocator<StatisticsService>.RegisterService(new StatisticsService());
        }
        
        private void UnregisterServices()
        {
            ServiceLocator<AudioService>.UnregisterService();
            ServiceLocator<SaveService>.UnregisterService();
            ServiceLocator<WaiterService>.UnregisterService();
            ServiceLocator<UITextService>.UnregisterService();
            ServiceLocator<UISayService>.UnregisterService();
            ServiceLocator<UIFaderService>.UnregisterService();
            ServiceLocator<StatisticsService>.UnregisterService();
        }

        private void OnDestroy()
        {
            UnregisterServices();
            _isRunning = false;
        }
    }
}