using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime._Game.Scripts.Runtime.CMS;
using Game.Runtime._Game.Scripts.Runtime.CMS.Components.Commons;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Services.Save;
using Game.Runtime._Game.Scripts.Runtime.Services.States;
using Game.Runtime.CMS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.Runtime._Game.Scripts.Runtime.Services.UI
{
    public class UIService : IService, IDisposable
    {
        private readonly Image _fadeImage;
        private readonly TMP_Text _title;
        private readonly float _fadeDuration = 1f;

        private CancellationTokenSource _fadeTokenSource;

        public UIService()
        {
            var uiServicePrefab = CMSProvider.GetEntity(CMSPrefabs.Services.UIService).GetComponent<PrefabComponent>().Prefab;
            var uiServiceObject = Object.Instantiate(uiServicePrefab);
            uiServiceObject.name = nameof(UIService);

            _fadeImage = uiServiceObject.GetComponentInChildren<Image>();
            _title = uiServiceObject.GetComponentInChildren<TMP_Text>();
        
            Object.DontDestroyOnLoad(uiServiceObject.gameObject);
            
            FadeOut().Forget();
        }
        
        public async UniTask FadeIn()
        {
            await Fade(0f, 1f);
        }

        public async UniTask FadeOut()
        {
            await Fade(1f, 0f);
        }

        public async UniTask ShowLevelTitle(int level)
        {
            var waiterService = ServicesProvider.GetService<WaiterService>();
            
            _title.gameObject.SetActive(true);
            await Print($"Day {level}");
            await waiterService.SmartWait(1.5f);
            await UnPrint();
            await waiterService.SmartWait(0.5f);
            _title.gameObject.SetActive(false);

            await FadeOut();
        }

        public async UniTask Print(string message)
        {
            _title.gameObject.SetActive(true);
            await ServicesProvider.GetService<UITextService>().Print(_title, message);
        }
        
        public async UniTask UnPrint()
        {
            await ServicesProvider.GetService<UITextService>().UnPrint(_title);
            _title.gameObject.SetActive(false);
        }
        
        
        private async UniTask Fade(float startAlpha, float endAlpha)
        {
            _fadeTokenSource?.Cancel();
            _fadeTokenSource = new CancellationTokenSource();

            try
            {
                float elapsed = 0f;
                Color fadeColor = _fadeImage.color;

                while (elapsed < _fadeDuration)
                {
                    if (_fadeTokenSource == null || _fadeTokenSource.IsCancellationRequested)
                        return;
                    
                    elapsed += Time.deltaTime;
                    fadeColor.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / _fadeDuration);
                    _fadeImage.color = fadeColor;
                
                    await UniTask.Yield(cancellationToken: _fadeTokenSource.Token)
                        .SuppressCancellationThrow();
                }

                fadeColor.a = endAlpha;
                _fadeImage.color = fadeColor;
            }
            finally
            {
                ResetFadeToken();
            }
        }

        private void ResetFadeToken()
        {
            _fadeTokenSource?.Dispose();
            _fadeTokenSource = null;
        }
        
        public void Dispose()
        {
            ResetFadeToken();
        }
    }
}