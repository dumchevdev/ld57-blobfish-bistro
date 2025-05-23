﻿using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Services.Audio;
using Game.Runtime._Game.Scripts.Runtime.Utils.Helpers;
using TMPro;

namespace Game.Runtime._Game.Scripts.Runtime.Services.UI
{
    public class UITextService : IService, IDisposable
    {
        private CancellationTokenSource _printTokenSource;
        
        public async UniTask Print(TMP_Text text, string message, 
            string fx = "wave", string audioEntityId = "")
        {
            _printTokenSource?.Cancel();
            _printTokenSource = new CancellationTokenSource();
            
            var visibleLength = Helpers.TextHelper.GetVisibleLength(message);
            if (visibleLength != 0)
            {
                try
                {
                    for (var i = 0; i < visibleLength; i++)
                    {
                        if ( _printTokenSource == null || _printTokenSource.IsCancellationRequested)
                            return;

                        text.text = $"<link={fx}>{Helpers.TextHelper.CutSmart(message, 1 + i)}</link>";
                        
                        await UniTask.WaitForEndOfFrame(cancellationToken: _printTokenSource.Token)
                            .SuppressCancellationThrow();

                        if (!string.IsNullOrEmpty(audioEntityId))
                        {
                            ServicesProvider.GetService<AudioService>().Play(audioEntityId);
                        }
                    }
                }
                finally
                {
                    ResetPrintToken();
                }
            }
        }
        
        public async UniTask UnPrint(TMP_Text text, string fx = "wave")
        {
            _printTokenSource?.Cancel();
            _printTokenSource = new CancellationTokenSource();
            
            var visibleLength = Helpers.TextHelper.GetVisibleLength(text.text);
            if (visibleLength != 0)
            {
                try
                {
                    for (var i = visibleLength - 1; i >= 0; i--)
                    {
                        if ( _printTokenSource == null || _printTokenSource.IsCancellationRequested)
                            return;

                        var str = Helpers.TextHelper.CutSmart(text.text, i);
                        text.text = $"<link={fx}>{str}</link>";
                        
                        await UniTask.WaitForEndOfFrame(cancellationToken: _printTokenSource.Token)
                            .SuppressCancellationThrow();
                    }
                }
                finally
                {
                    text.text = "";
                    ResetPrintToken();
                }
            }
        }

        private void ResetPrintToken()
        {
            _printTokenSource?.Dispose();
            _printTokenSource = null;
        }

        public void Dispose()
        {
            ResetPrintToken();
        }
    }
}