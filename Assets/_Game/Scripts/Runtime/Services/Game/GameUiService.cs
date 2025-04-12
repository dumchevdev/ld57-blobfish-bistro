using Game.Runtime._Game.Scripts.Runtime.CMS;
using Game.Runtime._Game.Scripts.Runtime.CMS.Components.Commons;
using Game.Runtime._Game.Scripts.Runtime.Gameplay;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime.CMS;
using Object = UnityEngine.Object;

namespace Game.Runtime._Game.Scripts.Runtime.Services.Game
{
    public class GameUiService : IService
    {
        private readonly GameUiBehaviour _gameUiBehaviour;
        
        public GameUiService()
        {
            var uiPrefab = CMSProvider.GetEntity(CMSPrefabs.Gameplay.UI).GetComponent<PrefabComponent>().Prefab;
            var uiObject = Object.Instantiate(uiPrefab);
            uiObject.name = nameof(GameUiService);
            
            _gameUiBehaviour = uiObject.GetComponent<GameUiBehaviour>();
        }

        public void Initialize()
        {
            ServiceLocator<SessionTimerService>.GetService().OnSessionTimerFinished += OnSessionTimerFinished;
        }

        public void SetActiveUIInput(bool active)
        {
            _gameUiBehaviour.SetActivateUIInput(active);
        }
        
        private void OnSessionTimerFinished()
        {
            _gameUiBehaviour.HandleSessionFinish();
            ServiceLocator<SessionTimerService>.GetService().OnSessionTimerFinished -= OnSessionTimerFinished;
        }
    }
}