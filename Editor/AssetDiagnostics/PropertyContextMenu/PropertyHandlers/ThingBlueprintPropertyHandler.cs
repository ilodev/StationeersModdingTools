using Assets.Scripts.Objects;
using Assets.Scripts.UI;
using ilodev.stationeers.moddingtools.diagnostics;
using ilodev.stationeers.moddingtools.uihelpers;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmodding.tools.diagnostics
{
    public class ThingBlueprintPropertyHandler : IPropertyContextMenuHandler
    {
        public void Register(PropertyContextMenuRegistry registry)
        {
            // Individual Slot property
            registry.RegisterHandler("Blueprint", (menu, property, target) =>
            {
                Thing thing = (Thing)target;

                menu.AddItem(new GUIContent("Generate blueprint prefab"), false, () =>
                {
                    thing.Blueprint = BuildBlueprintPrefab(thing);
                    EditorUtility.SetDirty(thing);
                });
            });

        }

        private GameObject BuildBlueprintPrefab(Thing thing) 
        {
            GameObject blueprintGO = new GameObject(thing.name + "_Blueprint");

            /// Add components
            var filter = blueprintGO.AddComponent<MeshFilter>();
            var renderer = blueprintGO.AddComponent<MeshRenderer>();

            string newNamespace = FindFirstAsmdefNamespace();
            string fullTypeName = newNamespace + ".Wireframe";

            Type type = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .FirstOrDefault(t => t.FullName == fullTypeName);

            var wireframe = blueprintGO.AddComponent(type) as Wireframe;
            wireframe.BlueprintTransform = thing.transform;
            wireframe.BlueprintMeshFilter = filter;
            wireframe.BlueprintRenderer = renderer;
            
            WireFrameInspector.GenerateWireframeEdgesAndCombinedMesh(wireframe, thing.transform);
            WireFrameInspector.SaveMeshFromObject(wireframe, "Meshes", blueprintGO.name, false);

            string blueprintFolder = "Assets/Blueprints";
            if (!AssetDatabase.IsValidFolder(blueprintFolder))
            {
                AssetDatabase.CreateFolder("Assets", "Blueprints");
            }

            // Create the prefab asset
            string prefabPath = Path.Combine(blueprintFolder, blueprintGO.name + ".prefab");
            prefabPath = AssetDatabase.GenerateUniqueAssetPath(prefabPath); // Avoid overwriting existing

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(blueprintGO, prefabPath);
            Debug.Log("Saved blueprint prefab to: " + prefabPath);

            // Destroy temporary scene object
            GameObject.DestroyImmediate(blueprintGO);

            // Return prefab reference if needed
            return prefab;
        }

        private string FindFirstAsmdefNamespace()
        {
            try
            {
                string[] guids = AssetDatabase.FindAssets("t:AssemblyDefinitionAsset", new[] { "Assets" });
                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    string json = File.ReadAllText(path);
                    return GetRootNamespaceFromJson(json);
                }
            }
            catch (Exception) { }
            return null;
        }

        private static string GetRootNamespaceFromJson(string json)
        {
            const string key = "\"rootNamespace\":";
            int index = json.IndexOf(key);

            if (index >= 0)
            {
                int startIndex = index + key.Length;
                int endIndex = json.IndexOf(",", startIndex);
                if (endIndex == -1) endIndex = json.IndexOf("}", startIndex);

                if (endIndex > startIndex)
                {
                    string value = json.Substring(startIndex, endIndex - startIndex).Trim().Trim('"');
                    return value;
                }
            }

            return null;
        }
    }
}
