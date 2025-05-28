using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Unity.VisualScripting.YamlDotNet.Core;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeers.moddingtools.installers
{
    public class QuickTipsWindow : EditorWindow
    {
        private Vector2 scrollPos;
        private bool showAssemblies;

        string[] targetAssemblies = {
            "Assembly-CSharp",
            "0Harmony",
            "BepInEx",
            "Unity.Mathematics",
            "Unity.Collections",
            "Unity.Burst",
        };

        [MenuItem("Window/Stationeers Modding Tools/QuickTips")]
        public static void ShowWindow()
        {
            GetWindow<QuickTipsWindow>("QuickTips");
        }

        private void OnGUI()
        {
            GUILayout.Label("QuickTips for MyPackage", EditorStyles.boldLabel);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            // Are we missing any assembly required for the project.
            ShowLoadedAssemblies();

            // Do we have an asmdef in our Assets/ folder.
            ShowAsmdef();

            EditorGUILayout.HelpBox("Install example assets", MessageType.Info);

            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Close"))
            {
                Close();
            }


        }

        /// <summary>
        /// Collapsible list of assembles required.
        /// </summary>
        private void ShowLoadedAssemblies()
        {
            Color oldColor = GUI.color;

            bool allAssembliesFound = CheckLoadedAssemblies();

            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            string assembliesFound = allAssembliesFound ? "All required assemblies found: Ok" : "Missing required assemblies";

            EditorGUILayout.Space();
        
            showAssemblies = EditorGUILayout.Foldout(showAssemblies, assembliesFound, true);

            if (showAssemblies)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                foreach (string assemblyName in targetAssemblies)
                {
                    var assembly = loadedAssemblies.FirstOrDefault(a => a.GetName().Name == assemblyName);
                    if (assembly != null)
                    {
                        GUI.color = Color.white;
                        var version = assembly.GetName().Version;
                        var test = assembly.GetName().FullName;
                        Debug.Log(assemblyName + " is loaded with version (v" + version + ") (" + test + ")");
                        GUILayout.Label($"{assemblyName}: {version}", EditorStyles.boldLabel);
                    }
                    else
                    {
                        GUI.color = Color.red;
                        Debug.LogWarning(assemblyName + " is not loaded.");
                        GUILayout.Label($"{assemblyName}: Missing", EditorStyles.boldLabel);
                    }
                }
                EditorGUILayout.EndVertical();
            }
            GUI.color = oldColor;
        }

        /// <summary>
        /// We could cache this, but we want the UI to update so we have to leave it in 
        /// the OnGUI call.
        /// </summary>
        /// <returns></returns>
        private bool CheckLoadedAssemblies()
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (string assemblyName in targetAssemblies)
            {
                var assembly = loadedAssemblies.FirstOrDefault(a => a.GetName().Name == assemblyName);
                if (assembly == null)
                    return false;
            }
            return true;
        }

        private void ShowAsmdef()
        {
            Color oldColor = GUI.color;
            EditorGUILayout.Space();

            bool oneAsmdefFound = CheckAsmdefs();

            string asmdefFound = oneAsmdefFound ? "At least one Asmdef in your Assets folder: Ok" : "Missing project Assembly definition";

            if (oneAsmdefFound)
            {
                GUI.color = Color.white;
            }
            else
            {
                GUI.color = Color.red;
            }
            GUILayout.Label(asmdefFound, EditorStyles.boldLabel);
            GUI.color = oldColor;
        }

        private bool CheckAsmdefs() {

            string[] guids = AssetDatabase.FindAssets("t:AssemblyDefinitionAsset", new[] { "Assets" });
            if (guids.Length == 0)
            {
                return false;
            }

            return true;
        }

    }
}
