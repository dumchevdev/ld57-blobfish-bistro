using Cysharp.Threading.Tasks;
using Game.Runtime.ServiceLocator;
using Game.Runtime.Services.UI;
using UnityEngine;

namespace Game.Runtime.Runners
{
    public class MainRunner : MonoBehaviour
    {
        private void Start()
        {
            StartGame().Forget();
        }

        private async UniTask StartGame()
        {
            var sayService = ServiceLocator<UISayService>.GetService();
            var faderService = ServiceLocator<UIFaderService>.GetService();

            await faderService.FadeOut();
            await sayService.Print("Wait for the jam to start...");
        }
    }
}