using Assets.Scripts.Objects;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.Linq;
using Assets.Scripts.Util;
using UnityEngine;

namespace ilodev.stationeersmods.tools.diagnostics
{
    /// <summary>
    /// Custom editor class for all classes inheriting Thing. This class will 
    /// collect all other editors and call them appropriately (there can only be
    /// one custom editor for each class type).
    /// </summary>
    [CustomEditor(typeof(Thing), true)] 
    public class ThingEditor : Editor
    {
        /// <summary>
        /// List of editors to be called before the original one.
        /// </summary>
        private IThingEditor[] m_beforeEditors;

        /// <summary>
        /// List of editors to be called after the original one.
        /// </summary>
        private IThingEditor[] m_afterEditors;

        /// <summary>
        /// Cache for contextual menu handlers.
        /// </summary>
        private static PropertyContextMenuRegistry contextMenuDict;

        /// <summary>
        /// Register itself to the Editor update event.
        /// </summary>
        private void OnEnable()
        {
            CollectPropertyHandlers();
            CollectEditors();
            OnEnableEditors();
            EditorApplication.update += OnUpdateEditors;
            EditorApplication.contextualPropertyMenu += OnPropertyContextMenu;
        }

        private void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property)
        {
            contextMenuDict.ExecuteHandlers(menu, property, target);

            foreach (var visualizer in m_beforeEditors)
                if (visualizer.GetType().GetMethod("OnPropertyContextMenu") != null)
                    visualizer.GetType().GetMethod("OnPropertyContextMenu").Invoke(visualizer, new object[] { menu, property, target });

            foreach (var visualizer in m_afterEditors)
                if (visualizer.GetType().GetMethod("OnPropertyContextMenu") != null)
                    visualizer.GetType().GetMethod("OnPropertyContextMenu").Invoke(visualizer, new object[] { menu, property, target });
        }

        private void CollectPropertyHandlers()
        {
            contextMenuDict = new PropertyContextMenuRegistry();

            var handlerTypes = TypeCache.GetTypesDerivedFrom<IPropertyContextMenuHandler>();
            foreach (var type in handlerTypes)
            {
                if (Activator.CreateInstance(type) is IPropertyContextMenuHandler handler)
                {
                    handler.Register(contextMenuDict);
                }
            }
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

        /// <summary>
        /// Enable all editors
        /// </summary>
        private void OnEnableEditors()
        {
            foreach (var editor in m_beforeEditors)
                editor.OnEnable(target);
            foreach (var editor in m_afterEditors)
                editor.OnEnable(target);
        }

        /// <summary>
        /// Deregister from the Editor update event.
        /// </summary>
        private void OnDisable()
        {
            OnDisableEditors();
            EditorApplication.update -= OnUpdateEditors;
            EditorApplication.contextualPropertyMenu -= OnPropertyContextMenu;
        }

        /// <summary>
        /// Disable all editors
        /// </summary>
        private void OnDisableEditors()
        {
            foreach (var editor in m_beforeEditors)
                editor.OnDisable(target);
            foreach (var editor in m_afterEditors)
                editor.OnDisable(target);
        }

        /// <summary>
        /// Run Update on all editors, mark dirty as appropriate
        /// </summary>
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

        /// <summary>
        /// Call all Before inspector GUI, and decide if the original 
        /// inspector needs to be called. 
        /// </summary>
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
