using Assets.Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.uihelpers
{

    [CustomEditor(typeof(Wireframe), true)]
    public class WireFrameInspector : Editor
    {
        /// <summary>
        /// Show the edges be drawn in the viewport
        /// </summary>
        private bool ShowEdges = false;

        /// <summary>
        /// Current inspector target
        /// </summary>
        private Wireframe wireframe;

        private Transform BlueprintSource;


        private void Awake()
        {
            wireframe = (Wireframe)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.LabelField("Modding tools", EditorStyles.boldLabel);

            /// Try wiring Transform, MeshFilter and MeshRenderer from the current gameobject 
            AutoAssignFromCurrentGameObject(wireframe);

            //BlueprintSource = GUILayout.

            BlueprintSource = (Transform)EditorGUILayout.ObjectField(
                new GUIContent("Source GameObject", "This is the object the tool will use to build the prefab mesh."),
                BlueprintSource, 
                typeof(Transform), 
                true
            );

            /// Cases to cover
            /// If there is no MeshFilter just ignore.
            /// if there is MeshFilter and has Mesh: Generate edges from current Mesh.
            /// if there is MeshFilter and has no Mesh: nothing
            /// - If there is BlueprintSource, and no mesh: generate edges and combined mesh from source.
            /// - if there is BlueprintSource, and mesh: generate edges from source.
            /// if there is Mesh and has no name: save current mesh

            if (BlueprintSource != null) {
                if (GUILayout.Button("Generate Edges and Mesh"))
                {
                    GenerateWireframeEdgesAndCombinedMesh(wireframe, wireframe.BlueprintTransform);
                    // Make dirty to force saving
                    EditorUtility.SetDirty(target);
                    Debug.Log("Generating Edges/Mesh finished");
                }
            } else
            {
                if (wireframe.BlueprintTransform != null)
                {
                    if (GUILayout.Button("Generate Edges"))
                    {
                        GenerateWireframeEdgesAndCombinedMesh(wireframe, wireframe.BlueprintTransform);
                        // Make dirty to force saving
                        EditorUtility.SetDirty(target);
                        Debug.Log("Generating Edges finished");
                    }
                }
            }


            ShowEdges = GUILayout.Toggle(ShowEdges, "Show Edges");
            if (wireframe.WireframeEdges.Count > 0)
            {
                if (GUILayout.Button("Save Mesh"))
                {
                    Debug.Log("Generating Edges starting");

                }
            }
        }

        public static void GenerateWireframeEdgesAndCombinedMesh(Wireframe wireframe, Transform target)
        {
            WireframeGenerator generator = new WireframeGenerator(wireframe.BlueprintTransform);
            wireframe.WireframeEdges = generator.Edges;
            if (wireframe.BlueprintMeshFilter != null)
                wireframe.BlueprintMeshFilter.sharedMesh = generator.CombinedMesh;
            wireframe.BlueprintBounds = generator.CombinedMesh.bounds;
        }


        /// <summary>
        /// Auto assign Transform, MeshFilter and MeshRenderer from the current 
        /// GameObject the component is attached to.
        /// </summary>
        /// <param name="wireframe"></param>
        private void AutoAssignFromCurrentGameObject(Wireframe wireframe)
        {
            /// Autoassign property if present in the same game object
            if (wireframe.BlueprintTransform == null)
                wireframe.BlueprintTransform = wireframe.transform;

            /// Autoassign property if present in the same game object
            if (wireframe.BlueprintMeshFilter == null)
                wireframe.BlueprintMeshFilter = wireframe.GetComponent<MeshFilter>();

            /// Autoassign property if present in the same game object
            if (wireframe.BlueprintRenderer == null)
                wireframe.BlueprintRenderer = wireframe.GetComponent<MeshRenderer>();
        }


        /// <summary>
        /// Used to draw the edges on the prefab/scene view
        /// </summary>
        public void OnSceneGUI()
        {
            if (ShowEdges == true)
            {
                DrawWireFrameEdges(wireframe.WireframeEdges, new Color(0, 255, 0, 0.8f));
                SceneView.RepaintAll();
            }
        }

        /// <summary>
        /// Draw a list of edges using Editor Handles
        /// </summary>
        /// <param name="edges">List of edges to draw</param>
        /// <param name="color">Color of the line</param>
        public static void DrawWireFrameEdges(List<Edge> edges, Color color)
        {
            Color prevColor = Handles.color;
            Handles.color = color;

            foreach (var edge in edges)
                Handles.DrawLine(edge.Point1, edge.Point2, 0.02f);

            Handles.color = prevColor;
        }

        private void OnDestroy()
        {
            Debug.Log("stop watching");
        }
    }
}
