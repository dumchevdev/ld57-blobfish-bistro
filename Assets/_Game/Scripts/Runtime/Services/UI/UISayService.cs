using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Runtime._Game.Scripts.Runtime.CMS;
using Game.Runtime._Game.Scripts.Runtime.CMS.Components.Commons;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime.CMS;
using TMPro;

namespace Game.Runtime._Game.Scripts.Runtime.Services.UI
{
    public class UISayService : IService
    {
        private readonly TMP_Text _text;
        private CancellationTokenSource _printTokenSource;
        
        public UISayService()
        {
            var sayPrefab = CMSProvider.GetEntity(CMSPrefabs.Services.SayText).GetComponent<PrefabComponent>().Prefab;
            var sayObject = UnityEngine.Object.Instantiate(sayPrefab);
            sayObject.name = nameof(UISayService);
            
            _text = sayObject.GetComponent<TMP_Text>();
            
            UnityEngine.Object.DontDestroyOnLoad(sayObject);
        }
        
        public async UniTask Print(string message, string fx = "wave")
        {
            await ServiceLocator<UITextService>.GetService().Print(_text, message, fx, CMSPrefabs.Services.SayText);
        }

        public async UniTask UnPrint(string fx = "wave")
        {
            await ServiceLocator<UITextService>.GetService().UnPrint(_text, fx);
        }
        
        public void AdjustSay(float i)
        {
            _text.transform.DOMoveY(i, 0.25f);
        }
    }
}