using Assets.Scripts.Objects;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.visualizers
{
    public class StructureGridBoundsVisualizer : IThingVisualizer
    {
        public void OnSceneGUI(SceneView sceneView, Object target)
        {
            Structure structure = target as Structure;
            if (structure == null)
                return;

            // Set color based on endpoint type
            Color color = Color.gray;
            float gridSize = structure.GridSize;
            float gridOffset = structure.GridOffset;

            // Define grid bounds (for example: 5x5 grid centered on object)
            int gridExtent = 3;

            Handles.color = new Color(0f, 1f, 1f, 0.2f); // cyan, semi-transparent

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
        }
    }
}
