using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.visualizers
{
    public class SmallGridBlockingVisualizer : IThingVisualizer
    {
        public void OnSceneGUI(SceneView sceneView, Object target)
        {
            if (!EditorPrefs.GetBool("Visualizer.GridBounds", true))
                return;

            Structure structure = target as Structure;
            if (structure == null)
                return;

            float GridSize = structure.GridSize;
            float GridBoundsRatio = structure.BoundsGridRatio;
            float SmallCellSize = 0.5f;

            float halfLarge = GridSize * 0.5f;
            float halfBound = halfLarge * GridBoundsRatio;

            // Draw large grid bounds for reference
            Handles.color = new Color(0.8f, 0.8f, 0.8f, 0.15f);
            Handles.DrawWireCube(Vector3.zero, Vector3.one * GridSize);

            // Max number of cells per axis you might cover
            int halfCellCount = Mathf.CeilToInt(halfLarge / SmallCellSize);

            for (int x = -halfCellCount; x <= halfCellCount; x++)
            {
                for (int y = -halfCellCount; y <= halfCellCount; y++)
                {
                    for (int z = -halfCellCount; z <= halfCellCount; z++)
                    {
                        Vector3 center = new Vector3(
                            x * SmallCellSize,
                            y * SmallCellSize,
                            z * SmallCellSize
                        );

                        // Check if this center is within the visual grid bounds
                        if (Mathf.Abs(center.x) <= halfBound &&
                            Mathf.Abs(center.y) <= halfBound &&
                            Mathf.Abs(center.z) <= halfBound)
                        {
                            // Draw blocked small grid cell
                            Handles.color = new Color(1f, 0f, 0f, 0.35f);
                            Handles.DrawWireCube(center, Vector3.one * SmallCellSize * 0.95f);
                        }
                    }
                }
            }


        }
    }
}
