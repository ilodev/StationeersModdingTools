using Assets.Scripts.Objects;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ilodev.stationeersmods.tools.visualizers
{
    public class StructureGridBoundsVisualizer : IThingVisualizer
    {
        public void OnSceneGUI(SceneView sceneView, Object target)
        {

            if (!EditorPrefs.GetBool("Visualizer.GridBounds", true))
                return;

            Structure structure = target as Structure;
            if (structure == null)
                return;

            Vector3 center = structure.transform.position;
            Vector3 size = Vector3.zero;

            if (structure.Bounds.size == Vector3.zero)
            {
                float gridSize = structure.GridSize;
                float gridRatio = structure.BoundsGridRatio;
                size = Vector3.one * gridSize * gridRatio;
            }
            else
            {
                center += structure.Bounds.center;
                size = structure.Bounds.size;
            }

            // Set color based on endpoint type
            Handles.color = new Color(0f, 1f, 1f, 0.3f); // cyan, semi-transparent
            Handles.DrawWireCube(center, size);
        }
    }
}
