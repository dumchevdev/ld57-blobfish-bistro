using UnityEditor;

namespace Game.Runtime.CMS.Editor
{
    public static class CMSMenuItems
    {
        [MenuItem("Tools/CMS/Reload")]
        public static void CMSReload()
        {
            CMSProvider.Unload();
            CMSProvider.Load();
        }
    }
}