using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public class NamespaceReplacer : EditorWindow
{
    private string oldNamespace = "Stationeers.ModdingHelpers";
    private string newNamespace = "NewNamespace";
    private int count = 0;

    [MenuItem("Window/Stationeers Modding Tools/Replace Namespace")]
    public static void ShowWindow()
    {
        GetWindow<NamespaceReplacer>("Replace Namespace");

        // Find asmdef namespace
    }

    void OnGUI()
    {
        GUILayout.Label("Namespace Replacement Tool", EditorStyles.boldLabel);
        oldNamespace = EditorGUILayout.TextField("Old Namespace", oldNamespace);
        newNamespace = EditorGUILayout.TextField("New Namespace", newNamespace);
        EditorGUILayout.LabelField("Total files changed: ", count.ToString());

        if (GUILayout.Button("Replace Namespaces"))
        {
            ReplaceNamespacesInAssets();
        }
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
