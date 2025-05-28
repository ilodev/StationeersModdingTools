using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditorInternal;
using System;

public class NamespaceReplacer : EditorWindow
{
    private string oldNamespace = "Stationeers.ModdingHelpers";
    private string newNamespace = "";

    private int count = 0;

    [MenuItem("Window/Stationeers Modding Tools/Replace Namespace")]
    public static void ShowWindow()
    {
        GetWindow<NamespaceReplacer>("Replace Namespace");
    }

    void OnGUI()
    {
        GUILayout.Label("Namespace Replacement Tool", EditorStyles.boldLabel);
        oldNamespace = EditorGUILayout.TextField("Old Namespace", oldNamespace);

        if (newNamespace == "")
            newNamespace = FindFirstAsmdefNamespace();

        newNamespace = EditorGUILayout.TextField("New Namespace", newNamespace);
        EditorGUILayout.LabelField("Total files changed: ", count.ToString());

        if (newNamespace != "" && GUILayout.Button("Replace Namespaces"))
        {
            ReplaceNamespacesInAssets();
        }
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
        return newNamespace;
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

    void ReplaceNamespacesInAssets()
    {
        string[] csFiles = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
        count = 0;

        foreach (string filePath in csFiles)
        {
            string content = File.ReadAllText(filePath);

            // Simple regex to match namespace definitions
            string pattern = $@"namespace\s+{Regex.Escape(oldNamespace)}";
            string replacement = $"namespace {newNamespace}";

            if (Regex.IsMatch(content, pattern))
            {
                string updatedContent = Regex.Replace(content, pattern, replacement);
                File.WriteAllText(filePath, updatedContent);
                count++;
            }
        }

        AssetDatabase.Refresh();
        Debug.Log($"Namespace replacement complete. Updated {count} file(s).");
    }
}
