using UnityEngine;

namespace Game.Runtime._Game.Scripts.Runtime.Utils.Extensions
{
    public static class ColorExtensions
    {
        public static Color SetAlpha(this Color c, float alpha)
        {
            return new Color(c.r, c.g, c.b, alpha);
        }
        
        public static Color ParseColor(this string str)
        {
            if (ColorUtility.TryParseHtmlString(str, out var clr))
                return clr;
            return UnityEngine.Color.magenta;
        }

        public static string Color(this string str, Color c)
        {
            return str.Color("#" + ColorUtility.ToHtmlStringRGBA(c));
        }
        
        public static string Color(this string str, string c)
        {
            string coloredString = "<color=" + c + ">";
            coloredString += str;
            coloredString += "</color>";
            return coloredString;
        }
    }
}