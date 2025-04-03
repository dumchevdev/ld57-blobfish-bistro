using Game.Runtime.CMS;
using Game.Runtime.ServiceLocator;
using Game.Runtime.Services.Audio;
using Game.Runtime.Services.Save;
using UnityEngine;

namespace Game.Runtime
{
    public class GameRunner : MonoBehaviour
    {
        private bool _isRunning;
        
        private void Awake()
        {
            if (_isRunning) return;
            
            CMSProvider.Load();
            
            RegisterServices();
            DontDestroyOnLoad(this);

            _isRunning = true;
        }

        private void RegisterServices()
        {
            var audioService = new AudioService(10);
            ServiceLocator<AudioService>.RegisterService(audioService);

            var saveService = new SaveService();
            ServiceLocator<SaveService>.RegisterService(saveService);
        }

        private void UnregisterServices()
        {
            ServiceLocator<AudioService>.UnregisterService();
            ServiceLocator<SaveService>.UnregisterService();
        }

        private void OnDestroy()
        {
            UnregisterServices();
            _isRunning = false;
        }
    }
}