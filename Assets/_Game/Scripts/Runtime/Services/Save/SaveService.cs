using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime.ServiceLocator;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.Runtime.Services.Save
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
            ResetCancellationToken();   
            _saveTokenSource = new CancellationTokenSource();
            
            while (true)
            {
                await UniTask.WaitForSeconds(1, cancellationToken: _saveTokenSource.Token);
                
                if (_saveTokenSource.IsCancellationRequested)
                    return;
                
                var str = JsonConvert.SerializeObject(SaveData);
                PlayerPrefs.SetString(SaveId, str);
            }
        }

        private void ResetCancellationToken()
        {
            _saveTokenSource?.Cancel();
            _saveTokenSource?.Dispose();
            _saveTokenSource = null;
        }

        public void Dispose()
        {
            ResetCancellationToken();
        }
    }
}