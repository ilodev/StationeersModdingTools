using Assets.Scripts.Objects;
using Objects.Rockets;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.diagnostics
{
    public class CustomSmallGridEditor : IThingEditor
    {
        public EditorType Type => EditorType.Before;

        public void OnDisable(Object target) { }

        public void OnEnable(Object target) { }

        public int OnInspectorGUI(Object target, int defaultHidden)
        {
            SmallGrid structureThing = target as SmallGrid;

            if (structureThing == null)
                return defaultHidden;

            if (structureThing.OpenEnds != null && structureThing.OpenEnds.Count >= 0)
            {
                bool showTransformErrors = false;
                bool showTypeErrors = false;
                foreach (Connection connection in structureThing.OpenEnds)
                {
                    if (connection.Transform == null)
                    {
                        showTransformErrors = true;
                    }
                    if (connection.ConnectionType == NetworkType.None)
                    {
                        showTypeErrors = true;
                    }
                }
                if (showTransformErrors) 
                    EditorGUILayout.HelpBox("OpenEnds without a transform parent.", MessageType.Error);
                if (showTypeErrors)
                    EditorGUILayout.HelpBox("OpenEnds without a type defined.", MessageType.Error);
            }
            return defaultHidden;
        }

        public int OnUpdate(Object target)
        {
            int result = 0;
            SmallGrid structureThing = target as SmallGrid;

            if (structureThing == null)
                return result;

            foreach (Connection connection in structureThing.OpenEnds)
            {
                if (connection.Parent == null)
                    connection.Parent = structureThing;
            }
            return result;
        }

    }
}
