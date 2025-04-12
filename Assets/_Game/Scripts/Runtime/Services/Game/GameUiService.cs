using System;
using Game.Runtime._Game.Scripts.Runtime.CMS;
using Game.Runtime._Game.Scripts.Runtime.CMS.Components.Commons;
using Game.Runtime._Game.Scripts.Runtime.CMS.Components.Gameplay;
using Game.Runtime._Game.Scripts.Runtime.Gameplay;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime.CMS;
using Object = UnityEngine.Object;

namespace Game.Runtime._Game.Scripts.Runtime.Services.Game
{
    public class GameUiService : IService, IDisposable
    {
        private readonly GameUiBehaviour _gameUiBehaviour;
        private readonly int _goal;
        
        public GameUiService()
        {
            var uiPrefab = CMSProvider.GetEntity(CMSPrefabs.Gameplay.UI).GetComponent<PrefabComponent>().Prefab;
            var uiObject = Object.Instantiate(uiPrefab);
            uiObject.name = nameof(GameUiService);
            
            _gameUiBehaviour = uiObject.GetComponent<GameUiBehaviour>();
            _goal = CMSProvider.GetEntity(CMSPrefabs.Gameplay.GameSettings).GetComponent<GameSettingsComponent>().GoldRequired;
            _gameUiBehaviour.InitializeGoalProgress(_goal);
        }

        public void Initialize()
        {
            ServiceLocator<SessionTimerService>.GetService().OnSessionTimerFinished += OnSessionTimerFinished;
            ServiceLocator<GameService>.GetService().GameData.OnGoldsChanged += OnGoldsChanged;
        }

        private void OnGoldsChanged(int golds)
        {
            _gameUiBehaviour.HandleGoalProgress(golds, _goal);
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

        public void Dispose()
        {
            ServiceLocator<GameService>.GetService().GameData.OnGoldsChanged -= OnGoldsChanged;
        }
    }
}