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
        public bool ShowEdge = false;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Wireframe wf = (Wireframe)target;

            if (GUILayout.Button("Generate Edges"))
            {
                Debug.Log("Generating Edges starting");
                WireframeGenerator gen = new WireframeGenerator(wf.BlueprintTransform);
                wf.WireframeEdges = gen.Edges;
                wf.BlueprintMeshFilter.sharedMesh = gen.CombinedMesh;
                // Make dirty to force saving
                EditorUtility.SetDirty(target);
                Debug.Log("Generating Edges finished");
            }

            ShowEdge = GUILayout.Toggle(ShowEdge, "Show Edges");
            if (wf.WireframeEdges.Count > 0)
            {
                if (GUILayout.Button("Save Mesh"))
                {
                    Debug.Log("Generating Edges starting");

                }
            }
        }

        public void OnSceneGUI()
        {
            if (ShowEdge == true)
            {
                Wireframe wf = (Wireframe)target;
                Color color = Handles.color;
                Handles.color = new Color(0, 255, 0, 0.8f);
                foreach (var edge in wf.WireframeEdges)
                {
                    Handles.DrawLine(edge.Point1, edge.Point2, 0.02f);
                }
                Handles.color = color;
                SceneView.RepaintAll();
            }

        }
    }
}
