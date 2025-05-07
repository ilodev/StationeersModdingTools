using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    // TODO: I need to centralize this creation into a custom util

    /// <summary>
    /// Creates a Constructor asset. If the Constructor script doesn't exist it will 
    /// create one. Once the assembly is compiled, it will create the game object and
    /// attach the right components to it
    /// </summary>
    [InitializeOnLoad]
    public class CreateMultiConstructor
    {
        private const string PendingFlagKey = "StationeersModdingTools_PendingMultiConstructor";

        private const string DEFAULT_NAME = "NewMultiConstructorAsset";
        private const string scriptsPath = "Assets/Scripts/Created/";

        [MenuItem("Assets/Create/Stationeers/Constructors/Multi Constructor", false, 1)]
        public static void CreateMultiConstructorAsset()
        {
            string nameSpace = AssemblyDefinitionHelpers.FindAsmdefNamespace();
            string assemblyName = AssemblyDefinitionHelpers.FindAsmdefAssemblyName();

            EditorApplication.update += CreateMultiConstructorObject;

            string content = FileUtils.GenerateScript("MultiConstructor", "Assets.Scripts.Objects.MultiConstructor", "Stationeers/Constructors/MultiConstructor", nameSpace);
            FileUtils.CreateTextFile(Path.Combine(scriptsPath, "MultiConstructor.cs"), content, true);

            EditorPrefs.SetBool(PendingFlagKey, true);
        }

        /// <summary>
        /// Constructor that survives a domain reload
        /// </summary>
        static CreateMultiConstructor()
        {
            // This runs on every domain reload
            if (EditorPrefs.GetBool(PendingFlagKey, false))
            {
                EditorApplication.update += CreateMultiConstructorObject;
            }
        }


        /// <summary>
        /// Creates the Constructor GameObject
        /// </summary>
        public static void CreateMultiConstructorObject()
        {
            if (EditorPrefs.GetBool(PendingFlagKey, false))
            {
                if (!EditorApplication.isCompiling)
                {
                    EditorPrefs.DeleteKey(PendingFlagKey);

                    GameObject go = new GameObject(DEFAULT_NAME);
                    go.AddComponent<MeshRenderer>();
                    go.AddComponent<MeshFilter>();
                    go.AddComponent<MeshCollider>();

                    // Attach our constructor Script
                    string nameSpace = AssemblyDefinitionHelpers.FindAsmdefNamespace();
                    string assemblyName = AssemblyDefinitionHelpers.FindAsmdefAssemblyName();
                    Type type = System.Type.GetType(nameSpace + ".MultiConstructor, " + assemblyName);
                    if (type != null) go.AddComponent(type);

                    // Update the selection to our current object
                    Selection.activeObject = go;

                    // We don't need updating again
                    EditorApplication.update -= CreateMultiConstructorObject;
                }
            }
        }
    }
}
