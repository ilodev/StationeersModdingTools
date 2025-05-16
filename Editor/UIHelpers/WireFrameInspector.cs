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
                WireframeGenerator gen = new WireframeGenerator(wf.transform);
                wf.WireframeEdges = gen.Edges;
                wf.BlueprintMeshFilter.sharedMesh = gen.CombinedMesh;
                // Make dirty to force saving
                EditorUtility.SetDirty(target);
                Debug.Log("Generating Edges finished");
            }

            ShowEdge = GUILayout.Toggle(ShowEdge, "Show Edges");
        }

        public void OnSceneGUI()
        {
            if (ShowEdge == true)
            {
                Wireframe wf = (Wireframe)target;
                foreach (var edge in wf.WireframeEdges)
                {
                    Handles.DrawLine(edge.Point1, edge.Point2, 0.02f);
                }

            }

        }
    }
}
