using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Runtime.CMS;
using Game.Runtime.Framework;
using Game.Runtime.Framework.Services.Camera;
using Game.Runtime.ServiceLocator;
using Game.Runtime.Services.Audio;
using Game.Runtime.Services.States;
using Game.Runtime.Services.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Runtime.Runners
{
    public class LogoRunner : MonoBehaviour
    {
        [SerializeField] private Camera logoCamera;
        [SerializeField] private TMP_Text logoText;
        
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
            var audioService = ServiceLocator<AudioService>.GetService();
            var waiterService = ServiceLocator<WaiterService>.GetService();
            var faderService = ServiceLocator<UIFaderService>.GetService();
            var cameraService = ServiceLocator<CameraService>.GetService();
            
            audioService.Play(CMSPrefabs.Audio.Ambient);
            
            if (GameSettings.SKIP_INTRO)
            {
                SceneManager.LoadScene(GameSettings.MAIN_SCENE);
                return;
            }
            
            logoText.alpha = 0f;
            logoText.text = "GAME BY DUMCHEVDEV";
            
            await waiterService.SmartWait(0.5f);
            
            audioService.Play(CMSPrefabs.Audio.SFX.SFXClick);
            cameraService.UIShake();
            
            logoText.DOFade(1f, 0.5f);

            await waiterService.SmartWait(1.5f);

            audioService.Play(CMSPrefabs.Audio.SFX.SFXClick);
            cameraService.UIShake();

            logoText.DOFade(0f, 1f);

            await waiterService.SmartWait(1f);

            logoText.text = "<size=50%>MADE IN 48 HOURS FOR\nLUDUM DARE 57</size>";

            audioService.Play(CMSPrefabs.Audio.SFX.SFXClick);
            cameraService.UIShake();

            logoText.DOFade(1f, 0.5f);

            await waiterService.SmartWait(3.5f);

            audioService.Play(CMSPrefabs.Audio.SFX.SFXClick);
            cameraService.UIShake();

            logoText.DOFade(0f, 1.5f);
            
            await waiterService.SmartWait(1.5f);
            
            await faderService.FadeIn();
            
            SceneManager.LoadScene(GameSettings.MAIN_SCENE);
        }

        private void OnDestroy()
        {
            UnregisterServices();
        }
    }
}