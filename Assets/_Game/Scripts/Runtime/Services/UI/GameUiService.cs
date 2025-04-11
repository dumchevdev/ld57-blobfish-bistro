using Game.Runtime._Game.Scripts.Runtime.CMS;
using Game.Runtime._Game.Scripts.Runtime.CMS.Components.Commons;
using Game.Runtime._Game.Scripts.Runtime.Gameplay;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime.CMS;
using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Services.UI
{
    public class GameUiService : IService
    {
        private readonly GameUiBehaviour _gameUiBehaviour;
        
        public GameUiService()
        {
            var uiPrefab = CMSProvider.GetEntity(CMSPrefabs.Services.UI).GetComponent<PrefabComponent>().Prefab;
            var uiObject = Object.Instantiate(uiPrefab);
            uiObject.name = nameof(GameUiService);
            
            _gameUiBehaviour = uiObject.GetComponent<GameUiBehaviour>();
            
            Object.DontDestroyOnLoad(_gameUiBehaviour);
        }

        public void SetActiveUIInput(bool active)
        {
            _gameUiBehaviour.SetActivateUIInput(active);
        }
    }
}