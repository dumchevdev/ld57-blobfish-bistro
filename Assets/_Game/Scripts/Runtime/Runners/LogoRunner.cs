using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Services.Audio;
using Game.Runtime._Game.Scripts.Runtime.Services.Camera;
using Game.Runtime._Game.Scripts.Runtime.Services.States;
using Game.Runtime._Game.Scripts.Runtime.Services.UI;
using Game.Runtime.CMS;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Runtime._Game.Scripts.Runtime.Runners
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
            
            logoText.alpha = 0f;
            logoText.text = "GAME BY DUMCHEVDEV";
            
            await waiterService.SmartWait(0.5f);
            
            audioService.Play(CMSPrefabs.Audio.SFX.SFXTyping);
            cameraService.UIShake();
            
            logoText.DOFade(1f, 0.5f);

            await waiterService.SmartWait(1.5f);

            audioService.Play(CMSPrefabs.Audio.SFX.SFXTyping);
            cameraService.UIShake();

            logoText.DOFade(0f, 1f);

            await waiterService.SmartWait(1f);

            logoText.text = "<size=50%>MADE IN 48 HOURS FOR\nLUDUM DARE 57</size>";

            audioService.Play(CMSPrefabs.Audio.SFX.SFXTyping);
            cameraService.UIShake();

            logoText.DOFade(1f, 0.5f);

            await waiterService.SmartWait(2f);

            audioService.Play(CMSPrefabs.Audio.SFX.SFXTyping);
            cameraService.UIShake();

            logoText.DOFade(0f, 1.5f);
            
            await waiterService.SmartWait(1.5f);
            
            await faderService.FadeIn();
            
            SceneManager.LoadScene("_Game/Scenes/Main");
        }

        private void OnDestroy()
        {
            UnregisterServices();
        }
    }
}