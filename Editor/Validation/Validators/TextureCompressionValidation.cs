using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace ilodev.stationeers.moddingtools.validation
{
    public class TextureCompressionValidator : IAssetValidator
    {
        public List<AssetIssue> Validate(string path, Object asset)
        {
            List<AssetIssue> issues = new();

            if (asset is Texture2D)
            {
                var importer = (TextureImporter)AssetImporter.GetAtPath(path);

                if (importer.textureCompression != TextureImporterCompression.CompressedHQ)
                {
                    issues.Add(new AssetIssue
                    {
                        asset = asset,
                        path = path,
                        category = "Textures",
                        description = "Texture not using high-quality compression",
                        fixAction = () =>
                        {
                            importer.textureCompression = TextureImporterCompression.CompressedHQ;
                            importer.SaveAndReimport();
                        }
                    });
                }
            }

            return issues;
        }
    }
}