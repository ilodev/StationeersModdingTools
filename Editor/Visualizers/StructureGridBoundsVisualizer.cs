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

            // Set color based on endpoint type
            Color color = Color.gray;
            float gridSize = structure.GridSize;
            float gridOffset = structure.GridOffset;
            float gridRatio = structure.BoundsGridRatio;

            Vector3 bounds = new Vector3(gridSize, gridSize, gridSize);
            Handles.color = new Color(0f, 1f, 1f, 0.3f); // cyan, semi-transparent
            Handles.DrawWireCube(structure.transform.position, bounds * gridRatio);

            /*
            // Define grid bounds (for example: 5x5 grid centered on object)
            int gridExtent = 3;


            Vector3 center = structure.transform.position;

            for (int x = -gridExtent; x <= gridExtent; x++)
            {
                for (int z = -gridExtent; z <= gridExtent; z++)
                {
                    // Calculate grid point position with offset
                    float px = center.x + (x * gridSize) + gridOffset;
                    float pz = center.z + (z * gridSize) + gridOffset;
                    Vector3 point = new Vector3(px, center.y, pz);

                    // Draw small sphere at grid point
                    Handles.SphereHandleCap(0, point, Quaternion.identity, 0.1f, EventType.Repaint);
                }
            }
            */
        }
    }
}
