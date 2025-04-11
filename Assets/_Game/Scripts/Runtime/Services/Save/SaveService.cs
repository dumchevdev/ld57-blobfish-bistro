using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Services.Save
{
    public class SaveService : IService, IDisposable
    {
        private const string SaveId = "SaveId";

        public SaveData SaveData;
        
        private CancellationTokenSource _saveTokenSource;

        public SaveService()
        {
            TryLoading();
            StartAutoSave();
        }

        private void TryLoading()
        {
            SaveData = JsonConvert.DeserializeObject<SaveData>(PlayerPrefs.GetString(SaveId, "{}"));
        }
        
        private async void StartAutoSave()
        {
            _saveTokenSource?.Cancel();
            _saveTokenSource = new CancellationTokenSource();

            try
            {
                while (true)
                {
                    if ( _saveTokenSource == null || _saveTokenSource.IsCancellationRequested)
                        return;
                    
                    await UniTask.WaitForSeconds(1, cancellationToken: _saveTokenSource.Token)
                        .SuppressCancellationThrow();
                    
                    var str = JsonConvert.SerializeObject(SaveData);
                    PlayerPrefs.SetString(SaveId, str);
                }
            }
            finally
            {
                ResetSaveToken();
            }
        }

        private void ResetSaveToken()
        {
            _saveTokenSource?.Dispose();
            _saveTokenSource = null;
        }

        public void Dispose()
        {
            ResetSaveToken();
        }
    }
}