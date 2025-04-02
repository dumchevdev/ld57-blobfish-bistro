using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime.CMS
{
    public class CMSEntityPrefab : MonoBehaviour
    {
        public string EntityId => GetEntityId();
        
        [SerializeReference, SubclassSelector]
        public List<CMSComponent> Components;

        public string GetEntityId()
        {
#if UNITY_EDITOR
            string path = UnityEditor.AssetDatabase.GetAssetPath(gameObject);
        
            if (path.StartsWith("Assets/_Game/Resources/") && path.EndsWith(".prefab"))
            {
                path = path.Substring("Assets/_Game/Resources/".Length);
                path = path.Substring(0, path.Length - ".prefab".Length);
            }

            return path;
#endif
        }
    }
}