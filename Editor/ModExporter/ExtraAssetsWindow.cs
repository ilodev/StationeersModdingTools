using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeers.moddingtools.exporter
{
    class ExtraAssetsWindow : EditorWindow
    {
        Vector2 scrollPosition;

        public static List<string> extraAssets { get; private set; }

        [MenuItem("StationeersMods/Show Copy Assets")]
        public static void ShowWindow()
        {
            var window = GetWindow<ExtraAssetsWindow>("Additional Copy Assets", typeof(ExporterEditorWindow));
            window.minSize = new Vector2(250, 350);
            extraAssets = AssetUtility.GetAssets("t:scene");
        }

        private void OnGUI()
        {
            var windowRect = new Rect(0, 0, Screen.width, Screen.height);
            scrollPosition = GUI.BeginScrollView(windowRect, scrollPosition, windowRect);
            foreach (var path in extraAssets)
            {
                EditorGUILayout.LabelField(path);
            }
            GUI.EndScrollView(true);
        }
    }
}
