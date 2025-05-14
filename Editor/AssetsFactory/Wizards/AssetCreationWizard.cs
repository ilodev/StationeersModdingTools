using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Items;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    public class AssetCreationWizard : EditorWindow
    {
        private SerializedObject serializedData;
        private AssetWizardData wizardData;

        private SerializedProperty scriptToAttachProp;
        private SerializedProperty meshesProp;
        private SerializedProperty namePrefix;
        private SerializedProperty createAsPrefabProp;
        private SerializedProperty prefabSavePathProp;
        private SerializedProperty constructorProp;

        [MenuItem("Tools/Asset Creation Wizard")]
        public static void ShowWindow()
        {
            GetWindow<AssetCreationWizard>("Asset Creation Wizard");
        }

        private void OnEnable()
        {
            wizardData = CreateInstance<AssetWizardData>();
            serializedData = new SerializedObject(wizardData);

            scriptToAttachProp = serializedData.FindProperty("scriptToAttach");
            meshesProp = serializedData.FindProperty("meshes");
            createAsPrefabProp = serializedData.FindProperty("createAsPrefab");
            prefabSavePathProp = serializedData.FindProperty("prefabSavePath");
            constructorProp = serializedData.FindProperty("constructorAsset");
            namePrefix = serializedData.FindProperty("namePrefix");
        }

        private void OnGUI()
        {
            serializedData.Update();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Component & Mesh Setup", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Asset type", GUILayout.Width(EditorGUIUtility.labelWidth));

            if (wizardData.scriptToAttach != null)
            {
                EditorGUILayout.LabelField(wizardData.scriptToAttach.name, EditorStyles.objectField, GUILayout.ExpandWidth(true));
            }
            else
            {
                EditorGUILayout.LabelField("None", EditorStyles.helpBox, GUILayout.ExpandWidth(true));
            }

            if (GUILayout.Button("Select", GUILayout.MaxWidth(80)))
            {
                MonoBehaviourScriptPicker.Show(script =>
                {
                    scriptToAttachProp.objectReferenceValue = script;
                    serializedData.ApplyModifiedProperties();
                }, new[] { "Assets/Scripts" });
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(namePrefix); // name Prefix
            EditorGUILayout.PropertyField(meshesProp, true);
            EditorGUILayout.Space();
            /*
            EditorGUILayout.LabelField("Prefab Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(createAsPrefabProp);

            // Custom folder picker UI
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(prefabSavePathProp, new GUIContent("Prefab Save Path"));
            if (GUILayout.Button("Select Folder", GUILayout.MaxWidth(100)))
            {
                string selectedFolder = EditorUtility.OpenFolderPanel("Select Prefab Folder", Application.dataPath, "");
                if (!string.IsNullOrEmpty(selectedFolder))
                {
                    if (selectedFolder.StartsWith(Application.dataPath))
                    {
                        string relativePath = "Assets" + selectedFolder.Substring(Application.dataPath.Length);
                        prefabSavePathProp.stringValue = relativePath;
                    }
                    else
                    {
                        Debug.LogWarning("Selected folder must be inside the Assets folder.");
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            */


            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Construction/Deconstruction", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(constructorProp, new GUIContent("Constructor"));
            if (EditorGUI.EndChangeCheck())
            {
                GameObject referencedObject = constructorProp.objectReferenceValue as GameObject;
                if (referencedObject != null)
                {
                    if (referencedObject.GetComponent<Constructor>() == null && referencedObject.GetComponent<MultiConstructor>() == null && referencedObject.GetComponent<DynamicThingConstructor>() == null)
                    {
                        Debug.LogWarning($"{referencedObject.name} does not have a valid constructor component.");
                        constructorProp.objectReferenceValue = null;
                    }
                }
            }
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Prefab", EditorStyles.boldLabel);
            // Inline section for "Create as Prefab" checkbox and folder path
            EditorGUILayout.BeginHorizontal();
            wizardData.createAsPrefab = EditorGUILayout.Toggle("Create Prefabs", wizardData.createAsPrefab);

            // Disable folder selection when "Create as Prefab" is unchecked
            GUI.enabled = wizardData.createAsPrefab;

            wizardData.prefabSavePath = EditorGUILayout.TextField(wizardData.prefabSavePath, GUILayout.Width(200));  // Folder text field

            if (GUILayout.Button("Select", GUILayout.Width(80)))  // Folder select button
            {
                string folderPath = EditorUtility.OpenFolderPanel("Select Folder to Save Prefabs", "Assets", "");
                if (!string.IsNullOrEmpty(folderPath))
                {
                    folderPath = FileUtil.GetProjectRelativePath(folderPath);
                    wizardData.prefabSavePath = folderPath;
                }
            }
            GUI.enabled = true; // Reset GUI.enabled to default

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (GUILayout.Button("Create Assets"))
            {
                CreateAssets();
            }

            serializedData.ApplyModifiedProperties();
        }

        private void CreateAssets()
        {
            if (wizardData.meshes == null || wizardData.meshes.Count == 0)
            {
                Debug.LogWarning("No meshes assigned.");
                return;
            }

            if (wizardData.scriptToAttach == null)
            {
                Debug.LogWarning("No script selected.");
                return;
            }

            Type scriptType = wizardData.scriptToAttach.GetClass();
            if (scriptType == null || !scriptType.IsSubclassOf(typeof(MonoBehaviour)))
            {
                Debug.LogWarning("Selected script is not derived from Thing.");
                return;
            }

            string savePath = wizardData.prefabSavePath;
            if (wizardData.createAsPrefab && !AssetDatabase.IsValidFolder(savePath))
            {
                Debug.LogWarning($"Save folder '{savePath}' does not exist.");
                return;
            }

            try
            {
                int total = wizardData.meshes.Count;

                for (int i = 0; i < total; i++)
                {
                    Mesh mesh = wizardData.meshes[i];
                    if (mesh == null)
                    {
                        Debug.LogWarning($"Mesh at index {i} is null, skipping.");
                        continue;
                    }

                    float progress = (float)i / (total - 1);
                    EditorUtility.DisplayProgressBar("Creating Assets", $"Processing {mesh.name} ({i + 1}/{total})", progress);

                    GameObject go = new GameObject($"{wizardData.namePrefix}{mesh.name}");

                    // Add MeshFilter and assign mesh
                    MeshFilter meshFilter = go.AddComponent<MeshFilter>();
                    meshFilter.sharedMesh = mesh;

                    // Add MeshRenderer
                    go.AddComponent<MeshRenderer>();

                    // Add the selected MonoBehaviour script as a component
                    go.AddComponent(scriptType);

                    if (wizardData.createAsPrefab)
                    {
                        string prefabPath = $"{savePath}/{go.name}.prefab";
                        prefabPath = AssetDatabase.GenerateUniqueAssetPath(prefabPath);
                        PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
                        Debug.Log($"Prefab created at: {prefabPath}");
                        // Destroy temp GameObject after prefab creation to avoid clutter
                        DestroyImmediate(go);
                    }
                    else
                    {
                        Debug.Log($"Created GameObject: {go.name} (not saved as prefab)");
                    }

                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Asset creation complete.");
        }

    }
}
