using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game.Runtime.CMS.Editor
{
    public static class CMSEntityPrefabCreateButton
    {
        [MenuItem("Assets/CMS/Create CMSEntityPrefab", priority = 0)]
        private static void CreateCMSEntityPrefab()
        {
            string folderPath = "Assets";
            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                folderPath = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(folderPath) && File.Exists(folderPath))
                {
                    folderPath = Path.GetDirectoryName(folderPath);
                }

                break;
            }

            GameObject newPrefab = new GameObject("NewCMSEntityPrefab");
            newPrefab.AddComponent<CMSEntityPrefab>();

            // Создаем префаб
            string prefabPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(folderPath, "CMSEntityPrefab.prefab"));
            PrefabUtility.SaveAsPrefabAsset(newPrefab, prefabPath);

            // Уничтожаем временный GameObject
            Object.DestroyImmediate(newPrefab);

            // Выделяем созданный префаб в Project window
            AssetDatabase.Refresh();
            Object createdPrefab = AssetDatabase.LoadAssetAtPath<Object>(prefabPath);
            Selection.activeObject = createdPrefab;
        }
    }
}