using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using Assets.Scripts.Objects;
using System.Reflection;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    public class PrefabCreator : Editor
    {
        private const string PendingFlagKey = "MyCustomTool_PendingScript";

        private static string nameScript = "NewBehaviourScript";

        [MenuItem("Tools/Start Prefab Creation Process")]
        static void StartProcess()
        {

            string nameSpace = AssemblyDefinitionHelpers.FindAsmdefNamespace();
            string assemblyName = AssemblyDefinitionHelpers.FindAsmdefAssemblyName();

            string scriptPath = "Assets/Scripts/Created/" + nameScript + ".cs";
            if (!File.Exists(scriptPath))
            {
                //string content = "using UnityEngine;\nusing Assets.Scripts.Objects;\n\nnamespace " + nameSpace + "\r\n{\r\npublic class " + nameScript + " : Constructor {\n}\r\n}";
                string content = FileUtils.GenerateScript(nameScript, "Assets.Scripts.Objects.Constructor", "Stationeers/Constructors/Constructor", nameSpace);
                File.WriteAllText(scriptPath, content);
                AssetDatabase.Refresh();
            }

            Debug.Log("Script created. Waiting for compile...");

            // Set a flag so we know to continue later
            EditorPrefs.SetBool(PendingFlagKey, true);
        }

        [InitializeOnLoadMethod]
        static void OnLoad()
        {
            // Hook into Editor update loop to watch for compilation completion
            EditorApplication.update += CheckCompileComplete;
        }

        static void CheckCompileComplete()
        {
            if (EditorPrefs.GetBool(PendingFlagKey, false))
            {
                if (!EditorApplication.isCompiling)
                {
                    EditorPrefs.DeleteKey(PendingFlagKey);

                    string nameSpace = AssemblyDefinitionHelpers.FindAsmdefNamespace();
                    string assemblyName = AssemblyDefinitionHelpers.FindAsmdefAssemblyName();

                    // Now create the GameObject and add the new component
                    GameObject go = new GameObject("NewPrefabWithScript");
                    go.AddComponent<MeshFilter>();
                    go.AddComponent<MeshRenderer>();
                    go.AddComponent<MeshCollider>();

                    Type type = System.Type.GetType(nameSpace + "." + nameScript+", " + assemblyName);

                    if (type != null)
                        go.AddComponent(type);
                    else
                        Debug.LogError("Type not found.");

                    Selection.activeObject = go;

                    string path = "Assets/Textures/NewPrefabWithScript.png";

                    CreateSprite(path);

                    Thing thing = go.GetComponent<Thing>(); 

                    // Get the importer for the asset
                    TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);

                    // If it's not a sprite yet, convert it
                    if (importer.textureType != TextureImporterType.Sprite)
                    {
                        importer.textureType = TextureImporterType.Sprite;
                        EditorUtility.SetDirty(importer);
                        importer.SaveAndReimport(); // Important: triggers reimport with new settings
                    }

                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                    Debug.Log($"Sprite {sprite}");
                    thing.Thumbnail = sprite;

                    /*
                    var component = go.AddComponent(type);
                    FieldInfo spriteField = type.GetField("Thumbnail", BindingFlags.Public | BindingFlags.Instance);
                    if (spriteField != null)
                    {
                        spriteField.SetValue(component, sprite);
                    }
                    */

                    Debug.Log("New prefab created and component added!");
                }
            }
        }


        static void CreateSprite(string path)
        {
            Texture2D tex = new Texture2D(128, 128);
            File.WriteAllBytes(path, tex.EncodeToPNG());
            AssetDatabase.ImportAsset(path);
        }
    }
}
