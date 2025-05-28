using System;
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
        /// Check namespace only in this assembly.
        /// </summary>
        private static string targetAssembly = "Assembly-CSharp.";

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
