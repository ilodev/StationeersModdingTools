using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.visualizers
{
    /// <summary>
    /// Helper class to support custom Editor Preferences
    /// </summary>
    public static class EditorPrefsHelper
    {
        /// <summary>
        /// Saves color as Hex string
        /// </summary>
        /// <param name="key"></param>
        /// <param name="color"></param>
        public static void SaveColor(string key, Color color)
        {
            string hex = ColorUtility.ToHtmlStringRGBA(color);
            EditorPrefs.SetString(key, hex);
        }

        /// <summary>
        /// Load Color from Hex String
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultColor"></param>
        /// <returns></returns>
        public static Color LoadColor(string key, Color defaultColor)
        {
            if (!EditorPrefs.HasKey(key))
                return defaultColor;

            string hex = EditorPrefs.GetString(key);
            if (ColorUtility.TryParseHtmlString("#" + hex, out Color color))
                return color;

            return defaultColor;
        }

        /// <summary>
        /// Save Color as separate RGBA floats
        /// </summary>
        /// <param name="key"></param>
        /// <param name="color"></param>
        public static void SaveColorRGBA(string key, Color color)
        {
            EditorPrefs.SetFloat(key + "_R", color.r);
            EditorPrefs.SetFloat(key + "_G", color.g);
            EditorPrefs.SetFloat(key + "_B", color.b);
            EditorPrefs.SetFloat(key + "_A", color.a);
        }

        /// <summary>
        /// Load Color from RGBA floats
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultColor"></param>
        /// <returns></returns>
        public static Color LoadColorRGBA(string key, Color defaultColor)
        {
            if (!EditorPrefs.HasKey(key + "_R"))
                return defaultColor;

            float r = EditorPrefs.GetFloat(key + "_R");
            float g = EditorPrefs.GetFloat(key + "_G");
            float b = EditorPrefs.GetFloat(key + "_B");
            float a = EditorPrefs.GetFloat(key + "_A");
            return new Color(r, g, b, a);
        }

        /// <summary>
        /// Delete stored color keys (Hex)
        /// </summary>
        /// <param name="key"></param>
        public static void DeleteColor(string key)
        {
            if (EditorPrefs.HasKey(key))
                EditorPrefs.DeleteKey(key);
        }

        /// <summary>
        /// Delete stored color keys (RGBA)
        /// </summary>
        /// <param name="key"></param>
        public static void DeleteColorRGBA(string key)
        {
            EditorPrefs.DeleteKey(key + "_R");
            EditorPrefs.DeleteKey(key + "_G");
            EditorPrefs.DeleteKey(key + "_B");
            EditorPrefs.DeleteKey(key + "_A");
        }
    }
}
