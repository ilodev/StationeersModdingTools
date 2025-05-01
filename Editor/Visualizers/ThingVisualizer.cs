using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Assets.Scripts.Objects;
using System.Linq;
using System;
using UnityEditor.SceneManagement;

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
            Thing[] containers = Resources.FindObjectsOfTypeAll<Thing>();

            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

            foreach (var container in containers)
            {
                // In Prefab Mode — only draw objects in the prefab scene
                if (prefabStage != null)
                {
                    if (container.gameObject.scene != prefabStage.scene)
                        continue;

                    foreach (var visualizer in m_ThingVisualizers)
                    {
                        visualizer.OnSceneGUI(sceneView, container);
                    }
                }
                
                // In regular scene — skip prefabs not in the active scene
                else
                {
                    if (!container.gameObject.scene.isLoaded)
                        continue;

                    SerializedObject so = new SerializedObject(container.gameObject);
                    SerializedProperty isActiveProp = so.FindProperty("m_IsActive");
                    if (isActiveProp.boolValue == true)
                    {
                        foreach (var visualizer in m_ThingVisualizers)
                        {
                            visualizer.OnSceneGUI(sceneView, container);
                        }

                    }
                  
                }
            }
        }
    }
}
