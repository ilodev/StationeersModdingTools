using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeers.moddingtools.installers
{
    public class CleanUpWindow : EditorWindow
    {
        private Vector2 scrollPos;
        private Dictionary<string, bool> assetSelection = new Dictionary<string, bool>();
        private bool removeEmptyFolders = true;
        private bool noResults = false;

        [MenuItem("Tools/Clean Up Window")]
        public static void ShowWindow()
        {
            GetWindow<CleanUpWindow>("Clean Up");
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Find Unused Assets"))
            {
                FindUnusedAssets();
            }

            if (assetSelection.Count > 0)
            {
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Select All")) SetAllSelection(true);
                if (GUILayout.Button("Select None")) SetAllSelection(false);
                if (GUILayout.Button("Invert Selection")) InvertSelection();
                GUILayout.EndHorizontal();

                GUILayout.Space(10);
                removeEmptyFolders = GUILayout.Toggle(removeEmptyFolders, "Remove Empty Folders After Deletion");

                GUILayout.Space(10);
                scrollPos = GUILayout.BeginScrollView(scrollPos);
                foreach (var asset in assetSelection.Keys.ToList())
                {
                    assetSelection[asset] = GUILayout.Toggle(assetSelection[asset], asset);
                }
                GUILayout.EndScrollView();

                GUILayout.Space(10);
                if (GUILayout.Button("Delete Selected Assets"))
                {
                    DeleteSelectedAssets();
                }
            }
            else if (noResults)
            {
                GUILayout.Space(10);
                EditorGUILayout.HelpBox("No unused assets found.", MessageType.Info);
            }
        }

        private void FindUnusedAssets()
        {
            assetSelection.Clear();
            noResults = false;

            string[] allAssetPaths = AssetDatabase.GetAllAssetPaths()
                .Where(path => path.StartsWith("Assets") && IsTargetAsset(path))
                .ToArray();

            Object[] sceneDependencies = EditorUtility.CollectDependencies(
                Resources.FindObjectsOfTypeAll(typeof(Object))
            );

            HashSet<string> usedAssets = new HashSet<string>();
            foreach (Object dependency in sceneDependencies)
            {
                string path = AssetDatabase.GetAssetPath(dependency);
                if (!string.IsNullOrEmpty(path)) usedAssets.Add(path);
            }

            foreach (string path in allAssetPaths)
            {
                if (!usedAssets.Contains(path))
                {
                    assetSelection[path] = false;
                }
            }

            if (assetSelection.Count == 0)
            {
                noResults = true;
            }
        }

        private bool IsTargetAsset(string path)
        {
            string ext = Path.GetExtension(path).ToLower();
            return ext == ".cs" || ext == ".mat" || ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".prefab" || ext == ".asset";
        }

        private void SetAllSelection(bool state)
        {
            var keys = assetSelection.Keys.ToList();
            foreach (var key in keys)
            {
                assetSelection[key] = state;
            }
        }

        private void InvertSelection()
        {
            var keys = assetSelection.Keys.ToList();
            foreach (var key in keys)
            {
                assetSelection[key] = !assetSelection[key];
            }
        }

        private void DeleteSelectedAssets()
        {
            var selected = assetSelection.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();
            foreach (var asset in selected)
            {
                AssetDatabase.DeleteAsset(asset);
                Debug.Log($"Deleted: {asset}");
            }

            AssetDatabase.Refresh();

            if (removeEmptyFolders)
            {
                RemoveEmptyFolders("Assets/");
            }

            FindUnusedAssets();
        }

        private void RemoveEmptyFolders(string rootPath)
        {
            var folders = Directory.GetDirectories(rootPath, "*", SearchOption.AllDirectories)
                .OrderByDescending(f => f.Length); // delete deeper paths first

            foreach (var folder in folders)
            {
                var relativePath = folder.Replace("\\", "/");
                if (AssetDatabase.IsValidFolder(relativePath))
                {
                    var files = Directory.GetFiles(relativePath)
                        .Where(f => !f.EndsWith(".meta"))
                        .ToArray();

                    var subfolders = Directory.GetDirectories(relativePath);

                    if (files.Length == 0 && subfolders.Length == 0)
                    {
                        AssetDatabase.DeleteAsset(relativePath);
                        Debug.Log($"Removed empty folder: {relativePath}");
                    }
                }
            }

            AssetDatabase.Refresh();
        }
    }
}
