using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeers.moddingtools.installers
{
    /// <summary>
    /// Class used to summarize missing dependencies or actions for A modding project.
    /// </summary>
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
            GUILayout.Label("Next steps to make a Stationeers Mod", EditorStyles.boldLabel);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            // Make sure the game libraries are available.
            CheckForGameAssemblies();

            // Check for missing packages
            CheckForMissingPackages();

            // If mod settings are not present then suggest making one
            CheckForModSettings();

            EditorGUILayout.HelpBox("Tip 1: You can access XYZ via the Tools menu.", MessageType.Info);
            EditorGUILayout.HelpBox("Tip 2: Remember to set up your layers before using ABC.", MessageType.Info);
            EditorGUILayout.HelpBox("Tip 3: Use the shortcut Ctrl+Alt+M to toggle feature DEF.", MessageType.Info);

            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Close"))
            {
                Close();
            }
        }

        #region Check for game assemblies, TODO: Refactor this into a shared function

        /// <summary>
        /// If the namespace is present, we will force this define for other asmdefs to know they 
        /// can be compiled.
        /// </summary>
        private static string defineSymbol = "STATIONEERS_DLL_PRESENT";  // Define symbol to control the assembly

        /// <summary>
        /// Suggest to install the game libraries if they are not found
        /// </summary>
        private void CheckForGameAssemblies()
        {
            if (!IsDefinePresent(defineSymbol))
                EditorGUILayout.HelpBox("Install game assemblies.", MessageType.Error);
        }

        /// <summary>
        /// Check if a define by name is present
        /// TODO: Separate a custom library (this function is duplicated now)
        /// </summary>
        /// <param name="define"></param>
        /// <returns></returns>
        public static bool IsDefinePresent(string define, BuildTargetGroup targetGroup = BuildTargetGroup.Standalone)
        {
            var buildTargetGroup = targetGroup;
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            return defines.Contains(define);
        }
        #endregion

        #region Check for Missing Packages
        private void CheckForMissingPackages()
        {
            // TODO CORRECT THE NAMESPACES REQUIRED
            // TODO Optionally replace all packages report with a button to install all the missing packages
            List<string> namespaces = new List<string>
            {
                "Unity.Mathematics",
                "Unity.Collections",
                "Unity.Burst",
                "UnityEngine.UI",
                "TMPro",
            };
            foreach (var name in namespaces) {
                if (!IsNamespacePresent(name))
                    EditorGUILayout.HelpBox($"Install missing package {name}.", MessageType.Error);
            }
        }

        /// <summary>
        /// Check if a namespace is present in the project: THIS IS A DUPLICATED FUNCTION
        /// </summary>
        /// <param name="namespaceName"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static bool IsNamespacePresent(string namespaceName, string filter = null)
        {
            // Get all assemblies loaded in the editor
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                // looking for a particular assembly name
                if (filter != null)
                    if (assembly.FullName.StartsWith(filter))
                        continue;

                try
                {
                    // Get all types in the assembly
                    var types = assembly.GetTypes();

                    // Check if any type belongs to the specified namespace
                    if (types.Any(t => t.Namespace == namespaceName))
                    {
                        // Debug.Log($"Namespace found in Assembly: {assembly}");
                        return true; // Namespace is found
                    }
                }
                catch (ReflectionTypeLoadException e)
                {
                    // Ignore any assemblies that fail to load types
                    Debug.LogWarning($"Failed to load types from assembly {assembly.FullName}. Error: {e.Message}");
                }
            }

            return false; // Namespace not found
        }


        #endregion

        #region Check for Mod Settings
        private void CheckForModSettings()
        {
            if (!IsDefinePresent(defineSymbol))
                return;

            EditorGUILayout.HelpBox("Define mod settings.", MessageType.Error);
        }
        #endregion


    }
}
