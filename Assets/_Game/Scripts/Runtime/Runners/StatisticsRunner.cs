using Cysharp.Threading.Tasks;
using Game.Runtime._Game.Scripts.Runtime.CMS;
using Game.Runtime._Game.Scripts.Runtime.CMS.Components.Gameplay;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Services.Camera;
using Game.Runtime._Game.Scripts.Runtime.Services.States;
using Game.Runtime._Game.Scripts.Runtime.Services.Statistics;
using Game.Runtime._Game.Scripts.Runtime.Services.UI;
using Game.Runtime._Game.Scripts.Runtime.Utils.Сonstants;
using Game.Runtime.CMS;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Runtime._Game.Scripts.Runtime.Runners
{
    public class StatisticsRunner : MonoBehaviour
    {
        [SerializeField] private Camera logoCamera;
        
        private void Start()
        {
            RegisterCamera();
            ShowStatistics().Forget();
        }

        private void RegisterCamera()
        {
            ServicesProvider.GetService<CameraService>().RegisterCamera(logoCamera);
        }

        private async UniTask ShowStatistics()
        {
            var statisticsService = ServicesProvider.GetService<StatisticsService>();
            var uiService = ServicesProvider.GetService<UIService>();
            var waiterService = ServicesProvider.GetService<WaiterService>();

            await ServicesProvider.GetService<UIService>().FadeOut();
            
            if (statisticsService.StatisticsData.CollectedGolds >= statisticsService.StatisticsData.Goal)
            {
                await uiService.Print("You win!");
            }
            else
            {
                await uiService.Print("You lose :'(");
            }
            
            await waiterService.SmartWait(0.5f);
            
            await waiterService.WaitClick();
            await uiService.UnPrint();
            await uiService.FadeIn();

            SceneManager.LoadScene(Const.ScenesConst.GameScene);
        }
    }
}