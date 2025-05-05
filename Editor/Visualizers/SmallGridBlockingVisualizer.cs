using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using static Assets.Scripts.Util.Defines;
using Color = UnityEngine.Color;

namespace ilodev.stationeersmods.tools.visualizers
{
    public class SmallGridBlockingVisualizer : IThingVisualizer
    {
        public float GridSize = 0.5f;
        public float GridOffset = 2.5f;

        public void OnSceneGUI(SceneView sceneView, Object target)
        {
            if (!EditorPrefs.GetBool("Visualizer.SmallGridBounds", true))
                return;

            Structure structure = target as Structure;
            if (structure == null)
                return;

            // Recalculate bounds if missing
            if (structure.Bounds.size == Vector3.zero)
                CachePrefabBounds(structure);

            // Get grid cells
            Vector3Int[] gridCells = GetSmallGridCellsForStructure(structure, GridSize);

            // Instantiate the prefab at each grid location
            foreach (Vector3Int gridIndex in gridCells)
            {
                // Convert the grid index to world position
                Vector3 worldPos = GridToWorldPosition(gridIndex, GridSize);

                Handles.color = new Color(1.0f, 0f, 0f, 0.5f); // red
                Handles.DrawWireCube(worldPos, Vector3.one * GridSize);
            }

        }

        public static void CachePrefabBounds(Structure structure)
        {
            structure.ThingTransform = structure.transform;
            Quaternion rotation = structure.ThingTransform.rotation;
            Vector3 position = structure.ThingTransform.position;
            structure.ThingTransform.rotation = Quaternion.identity;
            structure.ThingTransformPosition = Vector3.zero;
            Bounds bounds = structure.Bounds;
            structure.Bounds.center = Vector3.zero;
            structure.Bounds.extents = Vector3.zero;
            foreach (Renderer renderer in structure.GetComponentsInChildren<Renderer>())
            {
                if (!renderer.CompareTag("UIHelper"))
                    structure.Bounds.Encapsulate(renderer.bounds);
            }
            structure.ThingTransform.SetPositionAndRotation(position, rotation);
            structure.SurfaceArea = (float)(2.0 * ((double)structure.Bounds.size.x * (double)structure.Bounds.size.y + (double)structure.Bounds.size.y * (double)structure.Bounds.size.z + (double)structure.Bounds.size.z * (double)structure.Bounds.size.x)) * structure.SurfaceAreaScale;
        }

        // Your existing function to get small grid cells
        public static Vector3Int[] GetSmallGridCellsForStructure(Structure structure, float cellSize)
        {

            Bounds bounds = structure.Bounds;
            bounds.Expand(structure.BoundsExpand);

            Vector3 worldMin = bounds.min * structure.BoundsGridRatio;
            worldMin.y += structure.BoundsGridAddBottom;
            worldMin.x += worldMin.x * structure.BoundsGridExtraWidth;
            worldMin.z += worldMin.z * structure.BoundsGridExtraForward + structure.BoundsGridShiftForward;

            Vector3 worldMax = bounds.max * structure.BoundsGridRatio;
            worldMax.y += structure.BoundsGridAddHeight;
            worldMax.y += worldMax.y * structure.BoundsGridExtraHeight;
            worldMax.x += worldMax.x * structure.BoundsGridExtraWidth;
            worldMax.z += worldMax.z * structure.BoundsGridExtraForward;
            worldMax.z += worldMax.z * structure.BoundsForward;
            worldMax.z += structure.BoundsGridShiftForward;

            // Apply the structure's transform position to adjust the grid
            Vector3 transformPosition = structure.transform.position;

            // Shift the world min and max by the structure's world position
            worldMin += transformPosition;
            worldMax += transformPosition;

            Vector3Int gridMin = WorldToGridPosition(worldMin, cellSize);
            Vector3Int gridMax = WorldToGridPosition(worldMax, cellSize);

            int countX = Mathf.Abs(gridMax.x - gridMin.x) + 1;
            int countY = Mathf.Abs(gridMax.y - gridMin.y) + 1;
            int countZ = Mathf.Abs(gridMax.z - gridMin.z) + 1;

            Vector3Int[] gridCells = new Vector3Int[countX * countY * countZ];
            int index = 0;

            for (int x = 0; x < countX; x++)
            {
                for (int y = 0; y < countY; y++)
                {
                    for (int z = 0; z < countZ; z++)
                    {
                        Vector3Int cellIndex = new Vector3Int(
                            gridMin.x + x,
                            gridMin.y + y,
                            gridMin.z + z
                        );
                        gridCells[index++] = cellIndex;
                    }
                }
            }

            return gridCells;
        }

        /*
        // Convert a world position to grid index (rounded to nearest grid point)
        public static Vector3Int WorldToGridPosition(Vector3 worldPosition, float cellSize)
        {
            return new Vector3Int(
                Mathf.FloorToInt(worldPosition.x / cellSize),
                Mathf.FloorToInt(worldPosition.y / cellSize),
                Mathf.FloorToInt(worldPosition.z / cellSize)
            );
        }*/
        public static Vector3Int WorldToGridPosition(Vector3 worldPosition, float cellSize)
        {
            return new Vector3Int(
                Mathf.FloorToInt((worldPosition.x + cellSize / 2f) / cellSize),
                Mathf.FloorToInt((worldPosition.y + cellSize / 2f) / cellSize),
                Mathf.FloorToInt((worldPosition.z + cellSize / 2f) / cellSize)
            );
        }



        // Convert a grid index back to world position
        public static Vector3 GridToWorldPosition(Vector3Int gridPosition, float cellSize)
        {
            return new Vector3(
                gridPosition.x * cellSize,
                gridPosition.y * cellSize,
                gridPosition.z * cellSize
            );
        }

    }

}
