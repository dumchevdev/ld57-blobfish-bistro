using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Runtime._Game.Scripts.Runtime.CMS.Components.Commons;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Services.States;
using Game.Runtime._Game.Scripts.Runtime.Services.UI;

namespace Game.Runtime._Game.Scripts.Runtime.CMS.Entities.Scenarios
{
    public class TestScenarioEntity : CMSEntity
    {
        public TestScenarioEntity(string entityId, List<CMSComponent> components = null) : base(entityId, components)
        {
            Define<FuncComponent>().Func = StartScenario;
        }

        private async UniTask StartScenario()
        {
            var sayService = ServiceLocator<UISayService>.GetService();
            var waiterService = ServiceLocator<WaiterService>.GetService();
            var faderService = ServiceLocator<UIFaderService>.GetService();

            await faderService.FadeOut();
            await sayService.Print("Hello world!");
            await waiterService.SmartWait(10);
            await sayService.UnPrint();
            await waiterService.SmartWait(1);
            await sayService.Print("Again Hello world!");
            await waiterService.SmartWait(5);
            await sayService.UnPrint();
            await waiterService.SmartWait(1);
            sayService.AdjustSay(-1.25f);
            await waiterService.SmartWait(1);
            await sayService.Print("Some text");
            await faderService.FadeIn();
        }
    }
}