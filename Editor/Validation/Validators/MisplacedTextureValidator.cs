using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace ilodev.stationeers.moddingtools.validation
{
    public class MisplacedTextureValidator : IAssetValidator
    {
        public List<AssetIssue> Validate(string path, Object asset)
        {
            var issues = new List<AssetIssue>();

            if (asset is Texture2D && !path.Contains("/Textures/"))
            {
                issues.Add(new AssetIssue
                {
                    asset = asset,
                    path = path,
                    category = "Textures",
                    description = "Texture is not inside 'Textures/' folder",
                    fixAction = () =>
                    {
                        string newPath = "Assets/Textures/" + Path.GetFileName(path);
                        Directory.CreateDirectory("Assets/Textures");
                        string error = AssetDatabase.MoveAsset(path, newPath);
                        if (!string.IsNullOrEmpty(error))
                            Debug.LogError($"Failed to move asset: {error}");
                    }
                });
            }

            return issues;
        }
    }
}