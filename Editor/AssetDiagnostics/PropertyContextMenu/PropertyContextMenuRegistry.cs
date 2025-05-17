using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.diagnostics
{
    public class PropertyContextMenuRegistry
    {
        private Dictionary<int, List<Action<GenericMenu, SerializedProperty, UnityEngine.Object>>> handlers
            = new Dictionary<int, List<Action<GenericMenu, SerializedProperty, UnityEngine.Object>>>();

        public Dictionary<int, List<Action<GenericMenu, SerializedProperty, UnityEngine.Object>>> Handlers => handlers;

        public void RegisterHandler(string propertyName, Action<GenericMenu, SerializedProperty, UnityEngine.Object> action)
        {
            int hash = propertyName.GetHashCode();
            if (!handlers.TryGetValue(hash, out var list))
            {
                list = new List<Action<GenericMenu, SerializedProperty, UnityEngine.Object>>();
                handlers[hash] = list;
            }
            list.Add(action);
        }

        public void ExecuteHandlers(GenericMenu menu, SerializedProperty property, UnityEngine.Object target)
        {
            int hash = property.name.GetHashCode();
            if (handlers.TryGetValue(hash, out var list))
            {
                foreach (var action in list)
                {
                    action(menu, property, target);
                }
            }
        }
    }
}
