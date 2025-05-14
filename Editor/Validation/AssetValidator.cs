using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ilodev.stationeers.moddingtools.validation
{
    public static class AssetValidator
    {
        private static List<IAssetValidator> validators;

        static AssetValidator()
        {
            validators = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm => asm.GetTypes())
                .Where(t => typeof(IAssetValidator).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .Select(t => (IAssetValidator)Activator.CreateInstance(t))
                .ToList();
        }

        public static List<AssetIssue> ValidateAllAssets()
        {
            List<AssetIssue> issues = new();
            string[] allAssets = AssetDatabase.GetAllAssetPaths();

            foreach (string path in allAssets)
            {
                if (!path.StartsWith("Assets/")) continue;

                UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(path);
                if (asset == null) continue;

                foreach (var validator in validators)
                {
                    try
                    {
                        issues.AddRange(validator.Validate(path, asset));
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Validator error ({validator.GetType().Name}): {ex.Message}");
                    }
                }
            }

            return issues;
        }
    }
}
