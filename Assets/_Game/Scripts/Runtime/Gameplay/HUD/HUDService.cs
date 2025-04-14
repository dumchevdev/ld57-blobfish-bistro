using System;
using Game.Runtime._Game.Scripts.Runtime.CMS;
using Game.Runtime._Game.Scripts.Runtime.CMS.Components.Commons;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Services.Game;
using Game.Runtime.CMS;
using Object = UnityEngine.Object;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.HUD
{
    public class HUDService : IService, IInitializable, IDisposable
    {
        private readonly HUDBehaviour _hudBehaviour;
        private readonly float _goal;
        
        public HUDService()
        {
            var uiPrefab = CMSProvider.GetEntity(CMSPrefabs.Gameplay.HUD).GetComponent<PrefabComponent>().Prefab;
            var uiObject = Object.Instantiate(uiPrefab);
            uiObject.name = nameof(HUDService);
            
            _hudBehaviour = uiObject.GetComponent<HUDBehaviour>();
            _goal = ServicesProvider.GetService<GameService>().GetCurrentGoal();
            _hudBehaviour.InitializeGoalProgress(_goal);
        }

        public void Initialize()
        {
            ServicesProvider.GetService<SessionTimerService>().OnSessionTimerFinished += OnSessionTimerFinished;
            ServicesProvider.GetService<GameService>().GameData.OnGoldsChanged += OnGoldsChanged;
        }

        private void OnGoldsChanged(float golds)
        {
            _hudBehaviour.HandleGoalProgress(golds, _goal);
        }

        public void UpdateStatisticsPanel(string viewId)
        {
            _hudBehaviour.UpdateStatisticsPanel(viewId);
        }

        public void UpdateMoneyPanel(float money)
        {
            _hudBehaviour.UpdateMoneyPanel(money);
        }

        public void SetActiveUIInput(bool active)
        {
            _hudBehaviour.SetActivateUIInput(active);
        }
        
        private void OnSessionTimerFinished()
        {
            _hudBehaviour.HandleSessionFinish();
        }

        public void Dispose()
        {
            ServicesProvider.GetService<SessionTimerService>().OnSessionTimerFinished -= OnSessionTimerFinished;
            ServicesProvider.GetService<GameService>().GameData.OnGoldsChanged -= OnGoldsChanged;
        }
    }
}