using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Objects;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    public class MonoBehaviourScriptPicker : EditorWindow
    {
        private string searchQuery = "";
        private List<MonoScript> validScripts;
        private Vector2 scrollPos;

        public Action<MonoScript> onScriptSelected;

        public static void Show(Action<MonoScript> onScriptSelected, string[] searchFolders)
        {
            MonoBehaviourScriptPicker window = CreateInstance<MonoBehaviourScriptPicker>();
            window.titleContent = new GUIContent("Select MonoBehaviour");
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 600);
            window.onScriptSelected = onScriptSelected;
            window.LoadScripts(searchFolders);
            window.ShowUtility();
        }

        private void LoadScripts(string[] searchFolders)
        {
            string[] guids = AssetDatabase.FindAssets("t:MonoScript", searchFolders);

            validScripts = new List<MonoScript>();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                if (script != null)
                {
                    Type scriptClass = script.GetClass();
                    //if (scriptClass != null && scriptClass.IsSubclassOf(typeof(MonoBehaviour)))
                    if (scriptClass != null && scriptClass.IsSubclassOf(typeof(Assets.Scripts.Objects.Thing)))
                    {
                        validScripts.Add(script);
                    }
                }
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            searchQuery = GUILayout.TextField(searchQuery, GUI.skin.FindStyle("ToolbarSearchTextField"));
            if (GUILayout.Button("X", EditorStyles.toolbarButton, GUILayout.Width(20)))
            {
                searchQuery = "";
                GUI.FocusControl(null);
            }
            EditorGUILayout.EndHorizontal();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            foreach (MonoScript script in validScripts.Where(s => string.IsNullOrEmpty(searchQuery) || s.name.ToLower().Contains(searchQuery.ToLower())))
            {
                if (GUILayout.Button(script.name, EditorStyles.objectField))
                {
                    onScriptSelected?.Invoke(script);
                    Close();
                }
            }

            EditorGUILayout.EndScrollView();
        }
    }
}
