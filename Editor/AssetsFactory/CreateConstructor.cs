using Assets.Scripts.Objects;
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
    public class CreateConstructor
    {
        private const string PendingFlagKey = "StationeersModdingTools_PendingConstructor";

        private const string DEFAULT_NAME = "NewConstructorAsset";
        private const string scriptsPath = "Assets/Scripts/Created/";

        [MenuItem("Assets/Create/Stationeers/Constructors/Constructor", false, 1)]
        public static void CreateConstructorAsset()
        {
            string nameSpace = AssemblyDefinitionHelpers.FindAsmdefNamespace();
            string assemblyName = AssemblyDefinitionHelpers.FindAsmdefAssemblyName();

            EditorApplication.update += CreateConstructorObject;

            string content = FileUtils.GenerateScript("Constructor", "Assets.Scripts.Objects.Constructor", "Stationeers/Constructors/Constructor", nameSpace);
            FileUtils.CreateTextFile(Path.Combine(scriptsPath, "Constructor.cs"), content, true);

            EditorPrefs.SetBool(PendingFlagKey, true);
        }

        /// <summary>
        /// Constructor that survives a domain reload
        /// </summary>
        static CreateConstructor()
        {
            // This runs on every domain reload
            if (EditorPrefs.GetBool(PendingFlagKey, false))
            {
                EditorApplication.update += CreateConstructorObject;
            }
        }


        /// <summary>
        /// Creates the Constructor GameObject
        /// </summary>
        public static void CreateConstructorObject()
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

                    Type type = System.Type.GetType(nameSpace + ".Constructor, " + assemblyName);
                    if (type != null) go.AddComponent(type);

                    // Update the selection to our current object
                    Selection.activeObject = go;

                    // Add default stackable interactables
                    Thing thing = go.GetComponent<Thing>();
                    InteractableHelpers.AddInteractable(thing, "SplitOne", InteractableType.Button1);
                    InteractableHelpers.AddInteractable(thing, "SplitHalf", InteractableType.Button2);

                    // We don't need updating again
                    EditorApplication.update -= CreateConstructorObject;
                }
            }
        }
    }
}
