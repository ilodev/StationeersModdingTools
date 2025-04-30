using Assets.Scripts.Objects;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Assets.Scripts.Util;

namespace ilodev.stationeersmods.tools.diagnostics
{

    [CustomEditor(typeof(Thing), true)] // Works for all classes inheriting Thing
    public class ThingEditor : Editor
    {
        private IThingEditor[] m_beforeEditors;
        private IThingEditor[] m_afterEditors;

        private void OnEnable()
        {
            CollectEditors();
            OnEnableEditors();

            EditorApplication.update += OnUpdateEditors;
        }

        private void CollectEditors()
        {
            var types = TypeCache.GetTypesDerivedFrom(typeof(IThingEditor));

            var list = new List<IThingEditor>();

            foreach (var e in types)
            {
                var editor = (IThingEditor)Activator.CreateInstance(e);
                list.Add(editor);
            }

            m_beforeEditors = list
                .Where(t => t.Type == EditorType.Before)
                .ToArray();

            m_afterEditors = list
                .Where(t => t.Type == EditorType.After)
                .ToArray();
        }
        
        private void OnEnableEditors()
        {
            foreach (var editor in m_beforeEditors)
                editor.OnEnable(target);
            foreach (var editor in m_afterEditors)
                editor.OnEnable(target);
        }

        private void OnDisable()
        {
            OnDisableEditors();

            EditorApplication.update -= OnUpdateEditors;
        }

        private void OnDisableEditors()
        {
            foreach (var editor in m_beforeEditors)
                editor.OnDisable(target);
            foreach (var editor in m_afterEditors)
                editor.OnDisable(target);
        }

        private void OnUpdateEditors()
        {
            int setDirty = 0;
            foreach (var editor in m_beforeEditors)
                setDirty += editor.OnUpdate(target);
            foreach (var editor in m_afterEditors)
                setDirty += editor.OnUpdate(target);
            if (setDirty > 0)
                EditorUtility.SetDirty(target);
        }

        public override void OnInspectorGUI()
        {
            int result = OnInspectorGUIBefore();
            if (result == 0)
                base.OnInspectorGUI();
            OnInspectorGUIAfter(result);
        }

        private int OnInspectorGUIBefore()
        {
            int result = 0;
            foreach (var editor in m_beforeEditors)
                result += editor.OnInspectorGUI(target, result);
            return result;
        }

        private void OnInspectorGUIAfter(int defaultHidden)
        {
            foreach (var editor in m_afterEditors)
                editor.OnInspectorGUI(target, defaultHidden);
        }

    }
}
