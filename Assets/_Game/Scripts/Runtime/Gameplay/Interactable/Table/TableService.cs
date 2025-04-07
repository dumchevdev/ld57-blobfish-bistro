using System;
using System.Collections.Generic;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Commons;
using Game.Runtime.Gameplay.Character;
using Game.Runtime.ServiceLocator;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Runtime.Gameplay.Interactives
{
    public class TableService : IService
    {
        private readonly List<TableData> _tables;

        public TableService()
        {
            _tables = new List<TableData>();

            var tablesPrefab = CMSProvider.GetEntity(CMSPrefabs.Gameplay.Tables).GetComponent<PrefabComponent>().Prefab;
            var tableObject = Object.Instantiate(tablesPrefab);
            tableObject.name = nameof(TableService);
            
            var tableBehaviours = tableObject.GetComponentsInChildren<TableBehaviour>();
            for (int i = 0; i < tableBehaviours.Length; i++)
            {
                var tableBehaviour = tableBehaviours[i];
                tableBehaviour.SetId(i);
                tableBehaviour.SetStrategy(new EmptyTableInteraction());
                
                _tables.Add(new TableData(i, tableBehaviours[i]));
            }
        }

        public TableData GetTable(int tableId)
        {
            return _tables.Find(table => table.Id == tableId);
        }

        public void ShowAllTable(Color color, float width, bool blocked)
        {
            foreach (var tableData in _tables)
            {
                if (tableData.Client == null)
                    tableData.Behaviour.SetOutlineColor(color, width, blocked);
            }
        }
    }
}