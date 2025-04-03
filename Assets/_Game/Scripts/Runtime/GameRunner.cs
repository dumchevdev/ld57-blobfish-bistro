using Game.Runtime.CMS;
using Game.Runtime.ServiceLocator;
using Game.Runtime.Services.Audio;
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
            
            audioService.Play(CMSConstants.CMSEntityPrefab);
            Debug.Log($"{CMSProvider.GetEntity(CMSConstants.Test.TestEntity).EntityId}");
        }

        private void UnregisterServices()
        {
            ServiceLocator<AudioService>.UnregisterService();
        }

        private void OnDestroy()
        {
            UnregisterServices();
            _isRunning = false;
        }
    }
}