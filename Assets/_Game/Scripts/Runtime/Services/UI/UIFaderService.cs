using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime.ServiceLocator;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Services.UI
{
    public class UIFaderService : IService, IDisposable
    {
        private readonly Image _fadeImage;
        private readonly float _fadeDuration = 1f;

        private CancellationTokenSource _fadeTokenSource;

        public UIFaderService()
        {
            var canvas = new GameObject("ScreenFaderCanvas").AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 9;

            _fadeImage = new GameObject("FadeImage").AddComponent<Image>();
            _fadeImage.transform.SetParent(canvas.transform, false);
            
            _fadeImage.rectTransform.anchoredPosition = Vector2.zero;
            _fadeImage.rectTransform.sizeDelta = new Vector2(5000, 3000);

            _fadeImage.color = new Color(0f, 0f, 0f, 1f);
        
            UnityEngine.Object.DontDestroyOnLoad(canvas.gameObject);
        }
        
        public async UniTask FadeIn()
        {
            await Fade(0f, 1f);
        }

        public async UniTask FadeOut()
        {
            await Fade(1f, 0f);
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