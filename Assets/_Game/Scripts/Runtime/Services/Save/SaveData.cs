using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Services.Save
{
    public class SaveData
    {
        public int Level = 1;
        public StatisticsSaveData Statistics = new();
        public float VolumeSfx = 0.8f;
        public float VolumeMusic = 0.8f;
    }

    [Serializable]
    public class StatisticsSaveData
    {
        public float Money;
        
        public List<StatisticsTableData> TableData = new();

        public StatisticsTableData GetTableData(string viewId)
        {
            foreach (var tableData in TableData)
            {
                if (tableData.ViewId == viewId)
                    return tableData;
            }

            return null;
        }

        public void TryAddTableData(string viewId)
        {
            if (TableData.All(data => data.ViewId != viewId))
            {
                TableData.Add(new StatisticsTableData(viewId, 0));
                Debug.Log("ttestsetstset");
            }
        }
    }

    [Serializable]
    public class StatisticsTableData
    {
        public readonly string ViewId;
        public int Count;

        public StatisticsTableData(string viewId, int count)
        {
            ViewId = viewId;
            Count = count;
        }
    }
}