using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Assets.Scripts.Objects;
using System.Linq;
using System;

namespace ilodev.stationeersmods.tools.visualizers
{

    [InitializeOnLoad]
    public static class ThingVisualizer
    {
        static IThingVisualizer[] m_ThingVisualizers;

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

            // Static constructor subscribes to SceneView callback
        static ThingVisualizer()
        {
            CollectVisualizers();
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            Thing[] containers = GameObject.FindObjectsOfType<Thing>();

            foreach (var container in containers)
            {
                foreach(var visualizer in m_ThingVisualizers)
                {
                    visualizer.OnSceneGUI(sceneView, container);
                }
            }
        }
    }
}
