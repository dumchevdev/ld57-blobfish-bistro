using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime._Game.Scripts.Runtime.CMS;
using Game.Runtime._Game.Scripts.Runtime.CMS.Components.Gameplay;
using Game.Runtime._Game.Scripts.Runtime.Gameplay.HUD;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime.CMS;
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
        }

        public void UpdateStatisticsData(string viewId)
        {
            var statisticsData = SaveData.Statistics.GetTableData(viewId);
            statisticsData.Count++;
            
            ServicesProvider.GetService<HUDService>().UpdateStatisticsPanel(viewId);
        }

        private void TryLoading()
        {
            SaveData = JsonConvert.DeserializeObject<SaveData>(PlayerPrefs.GetString(SaveId, "{}"));
            FillStatisticsData();
        }
        
        private void FillStatisticsData()
        {
            var customers = CMSProvider.GetEntity(CMSPrefabs.Gameplay.Customer).GetComponent<CustomerViewsComponent>().Views;
            foreach (var customer in customers)
                SaveData.Statistics.TryAddTableData(customer.ViewId);

            var dishes = CMSProvider.GetEntity(CMSPrefabs.Gameplay.DishesLibrary).GetComponent<DishesLibraryComponent>().Dishes;
            foreach (var dinner in dishes)
                SaveData.Statistics.TryAddTableData(dinner.Id);
        }

        public void Save()
        {
            var str = JsonConvert.SerializeObject(SaveData);
            PlayerPrefs.SetString(SaveId, str);
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