using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Assets.Scripts.Objects;
using UnityEditor.SceneManagement;

namespace ilodev.stationeersmods.tools.visualizers
{
    /// <summary>
    /// Collects all Visualizers and calls OnSceneGUI on the required GameObjects
    /// </summary>
    [InitializeOnLoad]
    public static class ThingVisualizer
    {
        /// <summary>
        /// List of Classes implementing IThingVisualizer
        /// </summary>
        private static IThingVisualizer[] m_ThingVisualizers;

        /// <summary>
        /// Returns a list of available visualizers.
        /// </summary>
        /// <param name="Refresh"></param>
        /// <returns></returns>
        public static IThingVisualizer[] GetVisualizers( bool Refresh = false )
        {
            if (Refresh)
                m_ThingVisualizers = null;

            if (m_ThingVisualizers != null && m_ThingVisualizers.Length > 0)
                return m_ThingVisualizers;

            CollectVisualizers();

            return m_ThingVisualizers;
        }


        /// <summary>
        /// Find all classes implementing IThingVisualizer as populate the array
        /// </summary>
        private static void CollectVisualizers()
        {
            var types = TypeCache.GetTypesDerivedFrom(typeof(IThingVisualizer));

            var list = new List<IThingVisualizer>();

            foreach (var e in types)
            {
                var editor = (IThingVisualizer)Activator.CreateInstance(e);
                list.Add(editor);
            }

            m_ThingVisualizers = list.ToArray();
        }

        /// <summary>
        /// Constructor, Subscribes itself to the Scene GUI event
        /// </summary>
        static ThingVisualizer()
        {
            CollectVisualizers();
            SceneView.duringSceneGui += OnSceneGUI;
        }

        /// <summary>
        /// Calls all the OnSceneGUI visualizers of the active GameObjects
        /// in the current scene view.
        /// </summary>
        /// <param name="sceneView"></param>
        private static void OnSceneGUI(SceneView sceneView)
        {
            Thing[] containers = Resources.FindObjectsOfTypeAll<Thing>();

            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

            foreach (var container in containers)
            {
                if (prefabStage != null)
                {
                    // In Prefab Mode — only draw objects in the prefab scene.
                    // In this SceneView there is usually one Root object, that
                    // has to match the scene of the game object.
                    if (container.gameObject?.scene != prefabStage.scene)
                        continue;

                    foreach (var visualizer in m_ThingVisualizers)
                        visualizer.OnSceneGUI(sceneView, container);
                }
                else
                {
                    // In regular scene — skip prefabs not in the active scene
                    if (container.gameObject?.scene != null && !container.gameObject.scene.isLoaded)
                        continue;

                    // Find the IsActive property and disable all rendering for 
                    // elements that are not enabled in the hiearchy.
                    try
                    {
                        SerializedObject so = new SerializedObject(container.gameObject);
                        if (so.FindProperty("m_IsActive").boolValue == true)
                            foreach (var visualizer in m_ThingVisualizers)
                                visualizer.OnSceneGUI(sceneView, container);
                    }
                    catch (Exception) { }
                  
                }
            }
        }
    }
}
