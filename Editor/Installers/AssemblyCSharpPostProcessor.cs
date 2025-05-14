using UnityEditor;
using System.IO;
using System.Linq;
using System.Reflection;
using System;
using UnityEngine;

namespace ilodev.stationeers.moddingtools.installers
{
    /// <summary>
    /// Class to control a constraint for asmdef
    /// </summary>
    public class OptionalToolAssetPostProcessor : AssetPostprocessor
    {
        /// <summary>
        /// Check namespace. We use this namespace to control if the game assemblies are present or not.
        /// </summary>
        private static string targetNamespace = "Assets.Scripts.Objects";

        /// <summary>
        /// DLL file check. We use this dll modify the self-reference bit.
        /// </summary>
        private static string dllPath = "Assets/Assemblies/Assembly-CSharp.dll";  // Path to the DLL

        /// <summary>
        /// If the namespace is present, we will force this define for other asmdefs to know they 
        /// can be compiled.
        /// </summary>
        private static string defineSymbol = "STATIONEERS_DLL_PRESENT";  // Define symbol to control the assembly

        // Should we check for the dll file as well
        private static bool DllCheck = false;
        
        // Called when an asset is imported or deleted
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            // Check one of the known namespaces
            bool namespaceExists = IsNamespacePresent(targetNamespace);

            // Enable or disable tool based on the namespace existence
            if (namespaceExists)
            {
                AddDefineSymbol(defineSymbol);
                //Debug.Log($"{targetNamespace} found. Tool enabled.");
            }
            else
            {
                RemoveDefineSymbol(defineSymbol);
                //Debug.Log($"{targetNamespace} not found. Tool disabled.");
            }

            // Check if the DLL was added or removed
            if (DllCheck == true)
            {
                bool dllExists = File.Exists(dllPath);

                // If DLL exists or is newly added, ensure the define is added
                if (dllExists)
                {
                    AddDefineSymbol(defineSymbol);
                }
                // If DLL is removed, ensure the define is removed
                else
                {
                    RemoveDefineSymbol(defineSymbol);
                }
            }
        }

        /// <summary>
        /// Checks if a namespace is present in the loaded assemblies.
        /// </summary>
        /// <param name="namespaceName"></param>
        /// <returns></returns>
        public static bool IsNamespacePresent(string namespaceName)
        {
            // Get all assemblies loaded in the editor
            var assemblies = AppDomain.CurrentDomain.GetAssemblies(); 

            foreach (var assembly in assemblies)
            {
                // looking for a particular assembly name
                if (assembly.FullName.StartsWith("Assemby-CSharp."))
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

            // Only add the symbol if it doesn't already exist
            if (!defines.Contains(define))
            {
                defines = string.IsNullOrEmpty(defines) ? define : defines + ";" + define;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines);
                Debug.Log($"Added project level define: {define}");

                // Force an script reload
                EditorUtility.RequestScriptReload();
            }
        }

        /// <summary>
        /// Removes a define from the project. This define is used to constrain assembly definitions.
        /// </summary>
        /// <param name="define"></param>
        static void RemoveDefineSymbol(string define)
        {
            var buildTargetGroup = BuildTargetGroup.Standalone;
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

            // Only remove the symbol if it exists
            if (defines.Contains(define))
            {
                defines = string.Join(";", defines.Split(';').Where(d => d != define));
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines);
                Debug.Log($"Removed project level define: {define}");

                // Force an script reload
                EditorUtility.RequestScriptReload();
            }
        }
    }
}
