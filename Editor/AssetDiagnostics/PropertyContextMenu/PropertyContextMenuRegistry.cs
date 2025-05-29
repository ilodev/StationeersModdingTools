using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.Properties;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeers.moddingtools.diagnostics
{
    public class PropertyContextMenuRegistry
    {
        private Dictionary<int, List<Action<GenericMenu, SerializedProperty, UnityEngine.Object>>> handlers
            = new Dictionary<int, List<Action<GenericMenu, SerializedProperty, UnityEngine.Object>>>();

        // Read only 
        public Dictionary<int, List<Action<GenericMenu, SerializedProperty, UnityEngine.Object>>> Handlers => handlers;

        public void RegisterHandler(string propertyPath, Action<GenericMenu, SerializedProperty, UnityEngine.Object> action)
        {
            int hash = propertyPath.GetHashCode();
            if (!handlers.TryGetValue(hash, out var list))
            {
                list = new List<Action<GenericMenu, SerializedProperty, UnityEngine.Object>>();
                handlers[hash] = list;
            }
            list.Add(action);
        }

        string CleanPropertyPath(string path)
        {
            // Remove all occurrences of [number]
            if (path.Contains("Array.data"))
                return Regex.Replace(path, @"\[\d+\]", "");
            return path;

        }
        
        public void ExecuteHandlers(GenericMenu menu, SerializedProperty property, UnityEngine.Object target)
        {
            // Remove any trailing /[.*/] from the name first.

            int hash = CleanPropertyPath(property.propertyPath).GetHashCode(); // property.name.GetHashCode();
            Debug.Log($"Registry calling handlers for {property.propertyPath}");
            if (handlers.TryGetValue(hash, out var list))
            {
                foreach (var action in list)
                    action(menu, property, target);
            }
        }
    }
}
