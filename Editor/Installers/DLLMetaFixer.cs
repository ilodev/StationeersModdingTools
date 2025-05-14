/*
using UnityEditor;
using System.IO;
using UnityEngine;

namespace ilodev.stationeers.moddingtools.installers
{
    [InitializeOnLoad]
    public static class DllMetaFixer
    {
        static DllMetaFixer()
        {
            Debug.Log("Running META FIXER");
            EditorApplication.update += CheckAndFixDllMeta;
        }

        static void CheckAndFixDllMeta()
        {
            string dllPath = "Assets/Assemblies/Assembly-CSharp.dll";
            string metaPath = dllPath + ".meta";

            if (File.Exists(metaPath))
            {
                string metaText = File.ReadAllText(metaPath);
                if (metaText.Contains("isExplicitlyReferenced: 0"))
                {
                    metaText = metaText.Replace("isExplicitlyReferenced: 0", "isExplicitlyReferenced: 1");
                    File.WriteAllText(metaPath, metaText);
                    Debug.Log("[MetaFixer] Fixed isExplicitlyReferenced in meta file.");
                    AssetDatabase.ImportAsset(dllPath, ImportAssetOptions.ForceUpdate);
                }

                // Remove the handler so it doesn't run every frame
                EditorApplication.update -= CheckAndFixDllMeta;
            }
        }
    }
}
*/