using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.diagnostics
{
    public class CustomStructureThingEditor : IThingEditor
    {
        public EditorType Type => EditorType.Before;

        public void OnDisable(Object target) { }

        public void OnEnable(Object target) { }

        public int OnInspectorGUI(Object target, int defaultHidden)
        {
            Structure structureThing = target as Structure;

            if (structureThing == null)
                return defaultHidden;

            if (structureThing.BuildStates.Count == 0)
            {
                EditorGUILayout.HelpBox("This Structure needs at least one BuildState defined.", MessageType.Error);
            }
            return defaultHidden;
        }

        public int OnUpdate(Object target)
        {
            int result = 0;
            Structure structureThing = target as Structure;

            if (structureThing == null)
                return result;

            return result;
        }

    }
}
