using Cysharp.Threading.Tasks;
using Game.Runtime._Game.Scripts.Runtime.CMS;
using Game.Runtime._Game.Scripts.Runtime.CMS.Components.Gameplay;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Services.Camera;
using Game.Runtime._Game.Scripts.Runtime.Services.States;
using Game.Runtime._Game.Scripts.Runtime.Services.Statistics;
using Game.Runtime._Game.Scripts.Runtime.Services.UI;
using Game.Runtime.CMS;
using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Runners
{
    public class StatisticsRunner : MonoBehaviour
    {
        [SerializeField] private Camera logoCamera;
        
        private void Start()
        {
            RegisterServices();
            ShowLogo().Forget();
        }

        private void RegisterServices()
        {
            ServiceLocator<CameraService>.RegisterService(new CameraService(logoCamera));
        }

        private void UnregisterServices()
        {
            ServiceLocator<CameraService>.UnregisterService();
        }

        private async UniTask ShowLogo()
        {
            var statisticsService = ServiceLocator<StatisticsService>.GetService();
            var waiterService = ServiceLocator<WaiterService>.GetService();

            await ServiceLocator<UIFaderService>.GetService().FadeOut();
            
            if (statisticsService.GameData.Golds >= CMSProvider.GetEntity(CMSPrefabs.Gameplay.GameSettings).GetComponent<GameSettingsComponent>().GoldRequired)
            {
                await ServiceLocator<UISayService>.GetService().Print("You win!");
            }
            else
            {
                await ServiceLocator<UISayService>.GetService().Print("You looooose :(");
            }
            
            await waiterService.SmartWait(2f);

            await ServiceLocator<UISayService>.GetService().UnPrint();
            
            await waiterService.SmartWait(0.5f);

            await ServiceLocator<UISayService>.GetService().Print($"Gold - {statisticsService.GameData.Golds} / {CMSProvider.GetEntity(CMSPrefabs.Gameplay.GameSettings).GetComponent<GameSettingsComponent>().GoldRequired}");
            
            await waiterService.SmartWait(2f);

            await ServiceLocator<UISayService>.GetService().UnPrint();
            
            await waiterService.SmartWait(0.5f);
            
            await ServiceLocator<UISayService>.GetService().Print("Thank you for playing!");
        }

        private void OnDestroy()
        {
            UnregisterServices();
        }
    }
}