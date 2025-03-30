using UnityEditor;

namespace _Game.CMS.Editor
{
    public static class CMSMenuItems
    {
        [MenuItem("CMS/Reload")]
        public static void CMSReload()
        {
            CMS.Unload();
            CMS.Load();
        }
    }
}