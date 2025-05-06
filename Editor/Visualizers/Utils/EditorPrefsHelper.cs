using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.visualizers
{
    public static class EditorPrefsHelper
    {
        // Save Color as Hex String
        public static void SaveColor(string key, Color color)
        {
            string hex = ColorUtility.ToHtmlStringRGBA(color);
            EditorPrefs.SetString(key, hex);
        }

        // Load Color from Hex String
        public static Color LoadColor(string key, Color defaultColor)
        {
            if (!EditorPrefs.HasKey(key))
                return defaultColor;

            string hex = EditorPrefs.GetString(key);
            if (ColorUtility.TryParseHtmlString("#" + hex, out Color color))
                return color;

            return defaultColor;
        }

        // Save Color as separate RGBA floats
        public static void SaveColorRGBA(string key, Color color)
        {
            EditorPrefs.SetFloat(key + "_R", color.r);
            EditorPrefs.SetFloat(key + "_G", color.g);
            EditorPrefs.SetFloat(key + "_B", color.b);
            EditorPrefs.SetFloat(key + "_A", color.a);
        }

        // Load Color from RGBA floats
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

        // Delete stored color keys (Hex)
        public static void DeleteColor(string key)
        {
            if (EditorPrefs.HasKey(key))
                EditorPrefs.DeleteKey(key);
        }

        // Delete stored color keys (RGBA)
        public static void DeleteColorRGBA(string key)
        {
            EditorPrefs.DeleteKey(key + "_R");
            EditorPrefs.DeleteKey(key + "_G");
            EditorPrefs.DeleteKey(key + "_B");
            EditorPrefs.DeleteKey(key + "_A");
        }
    }
}
