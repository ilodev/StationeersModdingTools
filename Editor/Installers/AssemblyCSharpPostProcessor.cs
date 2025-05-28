using UnityEditor;
using System.Linq;
using System.Reflection;
using System;
using UnityEngine;

namespace ilodev.stationeers.moddingtools.installers
{
    /// <summary>
    /// Class to control the constraint for asmdefs in the project. It will look for the 
    /// 'Assets.Scripts.Objects' namespace in Assembly-CSharp.dll assembly if present and
    /// enable disable the constraint as necessary.
    /// 
    /// TODO: Move the namespace finding into its own class if we ever need a second
    /// namespace check.
    /// </summary>
    public class OptionalToolAssetPostProcessor : AssetPostprocessor
    {
        /// <summary>
        /// Check namespace. We use this namespace to control if the game assemblies are present or not.
        /// </summary>
        private static string targetNamespace = "Assets.Scripts.Objects";

        /// <summary>
        /// Check namespace only in this assembly.
        /// </summary>
        private static string targetAssembly = "Assembly-CSharp.";

        /// <summary>
        /// If the namespace is present, we will force this define for other asmdefs to know they 
        /// can be compiled.
        /// </summary>
        private static string defineSymbol = "STATIONEERS_DLL_PRESENT";  // Define symbol to control the assembly

        // Called when an asset is imported or deleted
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            // Enable or disable tool based on the namespace existence
            if (IsNamespacePresent(targetNamespace, targetAssembly))
            {
                if (!IsDefinePresent(defineSymbol))
                    AddDefineSymbol(defineSymbol);
            }
            else
            {
                if (IsDefinePresent(defineSymbol))
                    RemoveDefineSymbol(defineSymbol);
            }
        }

        /// <summary>
        /// Checks if a namespace is present in the loaded assemblies.
        /// </summary>
        /// <param name="namespaceName"></param>
        /// <returns>bool true if namespace is found</returns>
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

        /// <summary>
        /// Adds a define to the project. This define is used to constrain assembly definitions.
        /// </summary>
        /// <param name="define"></param>
        static void AddDefineSymbol(string define)
        {
            var buildTargetGroup = BuildTargetGroup.Standalone;
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

            defines = string.IsNullOrEmpty(defines) ? define : defines + ";" + define;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines);
            Debug.Log($"Added project level define: {define}");

            // Force an script reload
            EditorUtility.RequestScriptReload();
        }

        /// <summary>
        /// Removes a define from the project. This define is used to constrain assembly definitions.
        /// </summary>
        /// <param name="define"></param>
        static void RemoveDefineSymbol(string define)
        {
            var buildTargetGroup = BuildTargetGroup.Standalone;
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

            defines = string.Join(";", defines.Split(';').Where(d => d != define));
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines);
            Debug.Log($"Removed project level define: {define}");

            // Force an script reload
            EditorUtility.RequestScriptReload();
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

    }
}
