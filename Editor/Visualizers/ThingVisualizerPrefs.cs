using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.visualizers
{
    public class ThingVisualizerPrefs : EditorWindow
    {
        private Color openEndCellColor;

        private static readonly string openEndCellColorKey = "Visualizers.OpenEnds.CellColor";
        private static readonly Color DefaultOpenEndCellColor = new Color(1f, 0.65f, 0f, 1f); // Orange

        [MenuItem("Window/Stationeers Modding Tools/Visualizers")]
        public static void ShowWindow()
        {
            GetWindow<ThingVisualizerPrefs>("Stationeers Modding Tools Preferences");
        }

        private void OnEnable()
        {
            openEndCellColor = EditorPrefsHelper.LoadColor(openEndCellColorKey, DefaultOpenEndCellColor);
        }

        private void OnGUI()
        {
            GUILayout.Label("Preferences", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            openEndCellColor = EditorGUILayout.ColorField("OpenEnd Cell Color", openEndCellColor);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefsHelper.SaveColor(openEndCellColorKey, openEndCellColor);
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Reset to Default"))
            {
                openEndCellColor = DefaultOpenEndCellColor;
                EditorPrefsHelper.SaveColor(openEndCellColorKey, openEndCellColor);
            }
        }
    }
}