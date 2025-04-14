using System;
using System.Collections.Generic;
using System.Linq;
using Game.Runtime._Game.Scripts.Runtime.CMS;
using Game.Runtime._Game.Scripts.Runtime.CMS.Components.Gameplay;
using Game.Runtime._Game.Scripts.Runtime.ServiceLocator;
using Game.Runtime._Game.Scripts.Runtime.Services.Save;
using Game.Runtime.CMS;
using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Gameplay.HUD
{
    [Serializable]
    public class StatisticsTable
    {
        [SerializeField] private StatisticPanel[] customerPanels;
        [SerializeField] private StatisticPanel[] dinnerPanels;
        
        private readonly List<StatisticPanel> statisticPanels = new();

        public void InitializeStatistics()
        {
            var customers = CMSProvider.GetEntity(CMSPrefabs.Gameplay.Customer).GetComponent<CustomerViewsComponent>().Views;
            for (int i = 0; i < customers.Length; i++)
            {
                var customer = customers[i];
                var panel = customerPanels[i];
                panel.SetView(customer.ViewId, customer.Sprite);

                var saveService = ServicesProvider.GetService<SaveService>();
                var saveData = saveService.SaveData.Statistics.GetTableData(customer.ViewId);
                panel.UpdateCount(saveData.Count);
            }
            
            var dishes = CMSProvider.GetEntity(CMSPrefabs.Gameplay.DishesLibrary).GetComponent<DishesLibraryComponent>().Dishes;
            for (int i = 0; i < dinnerPanels.Length; i++)
            {
                var dinner = dishes[i];
                var panel = dinnerPanels[i];
                panel.SetView(dinner.Id, dinner.Sprite);

                var saveService = ServicesProvider.GetService<SaveService>();
                var saveData = saveService.SaveData.Statistics.GetTableData(dinner.Id);
                panel.UpdateCount(saveData.Count);
            }
            
            statisticPanels.AddRange(customerPanels);
            statisticPanels.AddRange(dinnerPanels);
        }

        public void UpdateStatisticsPanel(string viewId)
        {
            foreach (var panel in statisticPanels)
            {
                if (panel.ViewId == viewId)
                {
                    var count = ServicesProvider.GetService<SaveService>().SaveData.Statistics.GetTableData(viewId).Count;
                    panel.UpdateCount(count);
                }
            }
        }
    }
}