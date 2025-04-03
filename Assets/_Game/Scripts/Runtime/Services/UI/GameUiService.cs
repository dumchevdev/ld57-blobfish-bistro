using Game.Runtime.CMS;
using Game.Runtime.CMS.Commons;
using Game.Runtime.Gameplay;
using Game.Runtime.ServiceLocator;
using UnityEngine;

namespace Game.Runtime.Services.UI
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