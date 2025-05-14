using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace ilodev.stationeers.moddingtools.validation
{
    public class AssetNamingValidator : IAssetValidator
    {
        public List<AssetIssue> Validate(string path, Object asset)
        {
            var issues = new List<AssetIssue>();

            if (path.StartsWith("Assets/"))
            {
                string name = Path.GetFileNameWithoutExtension(path);

                if (name.Contains(" "))
                {
                    string newName = name.Replace(" ", "_");

                    issues.Add(new AssetIssue
                    {
                        asset = asset,
                        path = path,
                        category = "general",
                        description = "Asset name contains spaces",
                        fixAction = () =>
                        {
                            string error = AssetDatabase.RenameAsset(path, newName);
                            if (!string.IsNullOrEmpty(error))
                                Debug.LogError($"Failed to rename asset: {error}");
                        }
                    });
                }
            }
            return issues;
        }
    }
}
