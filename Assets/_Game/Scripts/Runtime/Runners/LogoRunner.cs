using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Services.Audio;
using Game.Runtime._Game.Scripts.Runtime.Services.Camera;
using Game.Runtime._Game.Scripts.Runtime.Services.States;
using Game.Runtime._Game.Scripts.Runtime.Services.UI;
using Game.Runtime._Game.Scripts.Runtime.Utils.Сonstants;
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
        [SerializeField] private SpriteRenderer logoRenderer;
        
        private void Start()
        {
            RegisterCamera();
            ShowLogo().Forget();
        }

        private void RegisterCamera()
        {
            ServicesProvider.GetService<CameraService>().RegisterCamera(logoCamera);
        }

        private async UniTask ShowLogo()
        {
            var audioService = ServicesProvider.GetService<AudioService>();
            var waiterService = ServicesProvider.GetService<WaiterService>();
            var faderService = ServicesProvider.GetService<UIService>();
            var cameraService = ServicesProvider.GetService<CameraService>();
            
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

            logoText.DOFade(0f, 1f);
            
            await waiterService.SmartWait(1.5f);

            logoText.text = "";
            logoRenderer.DOFade(1f, 2f);

            await waiterService.SmartWait(2f);

            logoRenderer.DOFade(0f, 2f);

            await waiterService.SmartWait(1f);

            await faderService.FadeIn();
            
            SceneManager.LoadScene(Const.ScenesConst.GameScene);
        }
    }
}