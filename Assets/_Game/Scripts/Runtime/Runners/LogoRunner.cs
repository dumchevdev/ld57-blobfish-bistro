using Cysharp.Threading.Tasks;
using Game.Runtime.CMS;
using Game.Runtime.Framework;
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
        [SerializeField] private TMP_Text _text;
        
        private void Start()
        {
            ShowLogo().Forget();
        }

        private async UniTask ShowLogo()
        {
            var audioService = ServiceLocator<AudioService>.GetService();
            var textService = ServiceLocator<UITextService>.GetService();
            var waiterService = ServiceLocator<WaiterService>.GetService();
            var faderService = ServiceLocator<UIFaderService>.GetService();
            
            audioService.Play(CMSPrefabs.Audio.Ambient);
            
            if (GameSettings.SKIP_INTRO)
            {
                SceneManager.LoadScene(GameSettings.MAIN_SCENE);
                return;
            }
            
            await waiterService.SmartWait(0.5f);
            audioService.Play(CMSPrefabs.Audio.SFX.SFXClick);
            
            await textService.Print(_text, "GAME BY DUMCHEVDEV", 
                audioEntityId: CMSPrefabs.Audio.SFX.SFXTyping);
            await waiterService.SmartWait(1.5f);

            audioService.Play(CMSPrefabs.Audio.SFX.SFXClick);
            
            await textService.UnPrint(_text);
            await waiterService.SmartWait(1f);

            audioService.Play(CMSPrefabs.Audio.SFX.SFXClick);
            
            await textService.Print(_text, "<size=50%>MADE IN 48 HOURS FOR\nLUDUM DARE 57</size>", 
                audioEntityId: CMSPrefabs.Audio.SFX.SFXTyping);
            await waiterService.SmartWait(1.5f);

            audioService.Play(CMSPrefabs.Audio.SFX.SFXClick);
            await textService.UnPrint(_text);

            await faderService.FadeIn();
            
            SceneManager.LoadScene(GameSettings.MAIN_SCENE);
        }
    }
}