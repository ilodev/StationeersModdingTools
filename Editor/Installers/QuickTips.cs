using UnityEditor;
using UnityEngine;

namespace ilodev.stationeers.moddingtools.installers
{
    public class QuickTipsWindow : EditorWindow
    {
        private Vector2 scrollPos;

        [MenuItem("Window/Stationeers Modding Tools/QuickTips")]
        public static void ShowWindow()
        {
            GetWindow<QuickTipsWindow>("QuickTips");
        }

        private void OnGUI()
        {
            GUILayout.Label("QuickTips for MyPackage", EditorStyles.boldLabel);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            EditorGUILayout.HelpBox("Tip 1: You can access XYZ via the Tools menu.", MessageType.Info);
            EditorGUILayout.HelpBox("Tip 2: Remember to set up your layers before using ABC.", MessageType.Info);
            EditorGUILayout.HelpBox("Tip 3: Use the shortcut Ctrl+Alt+M to toggle feature DEF.", MessageType.Info);

            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Close"))
            {
                Close();
            }
        }
    }
}
