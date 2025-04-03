using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Runtime.CMS.Commons;
using Game.Runtime.Services.States;
using Game.Runtime.ServiceLocator;
using Game.Runtime.Services.UI;


namespace Game.Runtime.CMS.Entities.Scenarios
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
            await waiterService.WaitMouseClick();
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