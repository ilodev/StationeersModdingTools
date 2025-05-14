using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Objects;

namespace ilodev.stationeers.moddingtools.validation
{
    public class PrefabMissingThumbnailValidator : IAssetValidator
    {
        public List<AssetIssue> Validate(string path, Object asset)
        {
            var issues = new List<AssetIssue>();

            if (asset is GameObject)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                DynamicThing dynamicThing;

                if (prefab != null && prefab.TryGetComponent<DynamicThing>(out dynamicThing))
                {
                    if (dynamicThing.Blueprint == null)
                    {
                        issues.Add(new AssetIssue
                        {
                            asset = asset,
                            path = path,
                            category = "Prefabs",
                            description = "Asset is missing a Blueprint",
                            fixAction = null
                        });
                    }
                    if (dynamicThing.Thumbnail == null)
                    {
                        issues.Add(new AssetIssue
                        {
                            asset = asset,
                            path = path,
                            category = "Prefabs",
                            description = "Asset is missing a Thumbnail",
                            fixAction = null
                        });
                    }
                }
            }

            return issues;
        }
    }
}
