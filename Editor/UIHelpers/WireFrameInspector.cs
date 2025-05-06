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

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Wireframe wf = (Wireframe)target;

            if (GUILayout.Button("Generate Edges"))
            {
                WireframeGenerator gen = new WireframeGenerator(wf.transform);
                wf.WireframeEdges = gen.Edges;

                // Make dirty to force saving
                EditorUtility.SetDirty(target);
            }

        }
    }
}
