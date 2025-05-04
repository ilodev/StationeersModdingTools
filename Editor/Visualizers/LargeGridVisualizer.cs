using Assets.Scripts.GridSystem;
using Assets.Scripts.Objects;
using Assets.Scripts.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.visualizers
{
    public class LargeGridVisualizer : IThingVisualizer
    {
        public void OnSceneGUI(SceneView sceneView, UnityEngine.Object target)
        {
            if (!EditorPrefs.GetBool("Visualizer.LargeGridBounds", true))
                return;

            Structure structure = target as Structure;
            if (structure == null)
                return;

            // Calculate bounds if not present
            if (structure.Bounds.size == Vector3.zero)
                CachePrefabBounds(structure); //CalculatePrefabBounds(structure);
            DrawWireBoxFromMinMax(structure.Bounds.center - structure.Bounds.extents, structure.Bounds.center + structure.Bounds.extents, Color.blue, true);
            //DrawCubeWithLabel(structure.Bounds.center, structure.Bounds.extents, Color.blue, true);


            // Overal grid bounds
            Bounds smallGridBounds = GetSmallGridBounds(structure);
            DrawWireBoxFromMinMax(smallGridBounds.min , smallGridBounds.max , Color.green, true);

            // Draw Large grids
            Grid3[] largeBounds = GetLocalGridBounds(structure);
            foreach (Grid3 grid3 in largeBounds)
            {
                DrawCubeWithLabel(grid3.ToVector3(), structure.GridSize, Color.white, true);
            }

            if (!EditorPrefs.GetBool("Visualizer.SmallGridBounds", true))
                return;
/*
            // Draw Small Grids
            Grid3[] bounds = GetLocalSmallGridBounds(structure);
            foreach (Grid3 grid3 in bounds)
            {
                //Debug.Log(grid3);
                DrawCubeWithLabel(grid3.ToVector3Raw()/10, 0.5f, new Color(1f, 0f, 0f, 1.0f), true);
            }
*/

            float cellSize = 0.5f; // Define your cell size
            Vector3Int[] gridCells = GetSmallGridCellsForStructure(structure, cellSize);
            foreach (Vector3Int gridIndex in gridCells)
            {
                // Convert grid index to world position
                Vector3 worldPos = GridToWorldPosition(gridIndex, cellSize);
                // You can now use the worldPos for your specific needs
                //Debug.Log($"Grid Index: {gridIndex}, World Position: {worldPos}");
                DrawCubeWithLabel(worldPos, cellSize, Color.red, false);
            }



        }

        void DrawCubeWithLabel(Vector3 centerPosition, float size, Color color, bool showLabel = false)
        {
            // Draw wire cube centered at position
            Handles.color = color;
            Handles.DrawWireCube(centerPosition, Vector3.one * size);

            // The label position can be nudged slightly in screen space if needed
            if (showLabel)
            {

                // Draw label at cube center
                GUIStyle labelStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    normal = { textColor = color },
                    fontSize = 10,
                    alignment = TextAnchor.MiddleCenter
                };

                Vector3 labelPos = centerPosition;
                Handles.Label(labelPos, $"({centerPosition.x:0.##}, {centerPosition.y:0.##}, {centerPosition.z:0.##})", labelStyle);
            }
        }


        void DrawWireBoxFromMinMax(Vector3 min, Vector3 max, Color color, bool showLabels = true)
        {
            Handles.color = color;

            Vector3 center = (min + max) * 0.5f;
            Vector3 size = max - min;

            // Draw wire cube
            Handles.DrawWireCube(center, size);

            if (showLabels)
            {
                GUIStyle labelStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    normal = { textColor = color },
                    fontSize = 9,
                    alignment = TextAnchor.UpperLeft
                };

                // Label at min point
                Handles.Label(min, $"Min: ({min.x:0.##}, {min.y:0.##}, {min.z:0.##})", labelStyle);

                // Label at max point
                Handles.Label(max, $"Max: ({max.x:0.##}, {max.y:0.##}, {max.z:0.##})", labelStyle);
            }
        }


        void DrawLarge3DGrid(Vector3 center, float cellSize, Vector3Int count, Color color)
        {
            Handles.color = color;

            Vector3 totalSize = new Vector3(count.x * cellSize, count.y * cellSize, count.z * cellSize);
            Vector3 startPos = center - totalSize * 0.5f + Vector3.one * (cellSize * 0.5f);

            for (int x = 0; x < count.x; x++)
            {
                for (int y = 0; y < count.y; y++)
                {
                    for (int z = 0; z < count.z; z++)
                    {
                        Vector3 pos = startPos + new Vector3(
                            x * cellSize,
                            y * cellSize,
                            z * cellSize
                        );

                        Handles.DrawWireCube(pos, Vector3.one * cellSize);
                        Vector3Int start = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z)) * 10;
                        string text = $"Grid: {start}";
                        Handles.Label(pos, text);
                    }
                }
            }
        }

        public static void DrawLarge3DGridExtend(Vector3 origin, float cellSize, Vector3Int gridDimensions, Color color)
        {
            Handles.color = color;

            for (int x = 0; x < gridDimensions.x; x++)
            {
                for (int y = 0; y < gridDimensions.y; y++)
                {
                    for (int z = 0; z < gridDimensions.z; z++)
                    {
                        Vector3 center = origin + new Vector3(
                            x * cellSize,
                            y * cellSize,
                            z * cellSize
                        );

                        Handles.DrawWireCube(center, Vector3.one * cellSize);
                        Vector3Int start = new Vector3Int(Mathf.FloorToInt(center.x), Mathf.FloorToInt(center.y), Mathf.FloorToInt(center.z)) * 10;
                        string text = $"Grid: {start}";
                        Handles.Label(center, text);
                    }
                }
            }
        }

        public static void DrawLarge3DGridAtOrigin(Vector3 origin, float cellSize, Vector3Int gridDimensions, Color color)
        {
            Handles.color = color;
            Vector3 halfCell = Vector3.one * (cellSize * 0.5f);

            GUIStyle labelStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 10,
                normal = { textColor = color },
                alignment = TextAnchor.MiddleCenter,
                richText = true
            };

            for (int x = 0; x < gridDimensions.x; x++)
            {
                for (int y = 0; y < gridDimensions.y; y++)
                {
                    for (int z = 0; z < gridDimensions.z; z++)
                    {
                        Vector3 center = origin + new Vector3(
                            x * cellSize,
                            y * cellSize,
                            z * cellSize
                        ) + halfCell;

                        // Draw the cube
                        Handles.DrawWireCube(center, Vector3.one * cellSize);
                        Vector3 cellOrigin = center - halfCell;
                        Vector3Int start = new Vector3Int(Mathf.FloorToInt(cellOrigin.x), Mathf.FloorToInt(cellOrigin.y), Mathf.FloorToInt(cellOrigin.z)) * 10;
                        string text = $"Grid3{start}";
                        Handles.Label(center, text);
                    }
                }
            }
        }


        public virtual void CalculatePrefabBounds(Structure structure)
        {
            Bounds BoundsBig = new Bounds();
            foreach (Grid3 grid3 in GetLocalGridBounds(structure))
            {
                Vector3 vector3 = grid3.ToVector3();
                BoundsBig.Encapsulate(vector3 + new Vector3(-1f, -1f, -1f));
                BoundsBig.Encapsulate(vector3 + new Vector3(1f, 1f, 1f));
            }
            structure.Bounds = BoundsBig;
            //return BoundsBig;
        }


        public virtual void CachePrefabBounds(Structure structure)
        {
            Bounds bounds = structure.Bounds;

            structure.Bounds.center = Vector3.zero;
            structure.Bounds.extents = Vector3.zero;

            foreach (Renderer renderer in structure.GetComponentsInChildren<Renderer>())
            {
                //if (!renderer.CompareTag("UIHelper"))
                structure.Bounds.Encapsulate(renderer.bounds);
            }

            structure.SurfaceArea = (float)(2.0 * ((double)structure.Bounds.size.x * (double)structure.Bounds.size.y + (double)structure.Bounds.size.y * (double)structure.Bounds.size.z + (double)structure.Bounds.size.z * (double)structure.Bounds.size.x)) * structure.SurfaceAreaScale;
        }


        public virtual Grid3[] GetLocalGridBounds(Structure structure)
        {
            if (structure.ForceGridBounds.Count > 0)
                return structure.ForceGridBounds.ToArray();
            Vector3 worldPosition1 = structure.Bounds.min * structure.BoundsGridRatio * structure.DualRegisterGridScale;
            worldPosition1.y += structure.BoundsGridAddBottom;
            worldPosition1.x += worldPosition1.x * structure.BoundsGridExtraWidth;
            worldPosition1.z += worldPosition1.z * structure.BoundsGridExtraForward;
            Vector3 worldPosition2 = structure.Bounds.max * structure.BoundsGridRatio * structure.DualRegisterGridScale;
            worldPosition2.y += structure.BoundsGridAddHeight;
            worldPosition2.y += worldPosition2.y * structure.BoundsGridExtraHeight;
            worldPosition2.x += worldPosition2.x * structure.BoundsGridExtraWidth;
            worldPosition2.z += (float)((double)worldPosition2.z * (double)structure.BoundsGridExtraForward + (double)worldPosition2.z * (double)structure.BoundsForward);
            Grid3 grid3_1 = ExtensionMethods.ToGrid(worldPosition1, 2f, 0.0f);
            Grid3 grid3_2 = ExtensionMethods.ToGrid(worldPosition2, 2f, 0.0f);
            Grid3 grid3_3 = grid3_1 + ExtensionMethods.ToGridPosition(structure.GridSize * 0.5f * Vector3.one);
            Grid3 grid3_4 = ExtensionMethods.ToGridPosition(structure.GridSize * 0.5f * Vector3.one);
            Grid3 grid3_5 = grid3_2 - grid3_4;
            float num1 = (float)((double)Math.Abs(grid3_5.x - grid3_3.x) / (double)structure.GridSize * 0.100000001490116);
            float num2 = (float)((double)Math.Abs(grid3_5.y - grid3_3.y) / (double)structure.GridSize * 0.100000001490116);
            float num3 = (float)((double)Math.Abs(grid3_5.z - grid3_3.z) / (double)structure.GridSize * 0.100000001490116);
            int num4 = 0;
            Grid3[] grid3Array = new Grid3[((int)num1 + 1) * ((int)num2 + 1) * ((int)num3 + 1)];
            for (int index1 = 0; (double)index1 <= (double)num1; ++index1)
            {
                for (int index2 = 0; (double)index2 <= (double)num2; ++index2)
                {
                    for (int index3 = 0; (double)index3 <= (double)num3; ++index3)
                    {
                        Grid3 grid3_6 = new Grid3((float)((double)index1 * (double)structure.GridSize * 10.0), (float)((double)index2 * (double)structure.GridSize * 10.0), (float)((double)index3 * (double)structure.GridSize * 10.0));
                        grid3_6 += grid3_3;
                        grid3Array[num4++] = grid3_6;
                    }
                }
            }
            return grid3Array;
        }

        /*
        public virtual Grid3[] GetLocalSmallGridBounds(Structure structure)
        {
            Bounds bounds = structure.Bounds;
            bounds.Expand(structure.BoundsExpand);
            Vector3 worldPosition1 = bounds.min * structure.BoundsGridRatio;
            worldPosition1.y += structure.BoundsGridAddBottom;
            worldPosition1.x += worldPosition1.x * structure.BoundsGridExtraWidth;
            worldPosition1.z += worldPosition1.z * structure.BoundsGridExtraForward + structure.BoundsGridShiftForward;
            Vector3 worldPosition2 = bounds.max * structure.BoundsGridRatio;
            worldPosition2.y += structure.BoundsGridAddHeight;
            worldPosition2.y += worldPosition2.y * structure.BoundsGridExtraHeight;
            worldPosition2.x += worldPosition2.x * structure.BoundsGridExtraWidth;
            worldPosition2.z += (float)((double)worldPosition2.z * (double)structure.BoundsGridExtraForward + (double)worldPosition2.z * (double)structure.BoundsForward) + structure.BoundsGridShiftForward;
            Grid3 grid3_1 = MyGrid3(worldPosition1);
            Grid3 grid3_2 = MyGrid3(worldPosition2);
            float num1 = (float)((double)Math.Abs(grid3_2.x - grid3_1.x) / 0.5 * 0.100000001490116);
            float num2 = (float)((double)Math.Abs(grid3_2.y - grid3_1.y) / 0.5 * 0.100000001490116);
            float num3 = (float)((double)Math.Abs(grid3_2.z - grid3_1.z) / 0.5 * 0.100000001490116);
            int num4 = 0;
            Grid3[] grid3Array = new Grid3[((int)num1 + 1) * ((int)num2 + 1) * ((int)num3 + 1)];
            for (int index1 = 0; (double)index1 <= (double)num1; ++index1)
            {
                for (int index2 = 0; (double)index2 <= (double)num2; ++index2)
                {
                    for (int index3 = 0; (double)index3 <= (double)num3; ++index3)
                    {
                        Grid3 grid3_3 = new Grid3((float)((double)index1 * 0.5 * 10.0), (float)((double)index2 * 0.5 * 10.0), (float)((double)index3 * 0.5 * 10.0));
                        grid3_3 += grid3_1;
                        grid3Array[num4++] = grid3_3;
                    }
                }
            }
            return grid3Array;
        }
        */


        public static Vector3Int[] GetSmallGridCellsForStructure(Structure structure, float cellSize)
        {
            Bounds bounds = structure.Bounds;
            bounds.Expand(structure.BoundsExpand);

            // Transform min world position
            Vector3 worldMin = bounds.min * structure.BoundsGridRatio;
            worldMin.y += structure.BoundsGridAddBottom;
            worldMin.x += worldMin.x * structure.BoundsGridExtraWidth;
            worldMin.z += worldMin.z * structure.BoundsGridExtraForward + structure.BoundsGridShiftForward;

            // Transform max world position
            Vector3 worldMax = bounds.max * structure.BoundsGridRatio;
            worldMax.y += structure.BoundsGridAddHeight;
            worldMax.y += worldMax.y * structure.BoundsGridExtraHeight;
            worldMax.x += worldMax.x * structure.BoundsGridExtraWidth;
            worldMax.z += worldMax.z * structure.BoundsGridExtraForward;
            worldMax.z += worldMax.z * structure.BoundsForward;
            worldMax.z += structure.BoundsGridShiftForward;

            // Convert world positions to grid indices (using Vector3Int for grid positions)
            Vector3Int gridMin = WorldToGridPosition(worldMin, cellSize);
            Vector3Int gridMax = WorldToGridPosition(worldMax, cellSize);

            // Calculate the number of positions along each axis (inclusive)
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
                        // Ensure each cell starts exactly where the previous one ended
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

        public virtual Grid3[] GetLocalSmallGridBounds(Structure structure)
        {
            Bounds bounds = structure.Bounds;
            bounds.Expand(structure.BoundsExpand);

            float cellSize = 0.5f;

            // Transform min world position
            Vector3 worldMin = bounds.min * structure.BoundsGridRatio;
            worldMin.y += structure.BoundsGridAddBottom;
            worldMin.x += worldMin.x * structure.BoundsGridExtraWidth;
            worldMin.z += worldMin.z * structure.BoundsGridExtraForward + structure.BoundsGridShiftForward;

            // Transform max world position
            Vector3 worldMax = bounds.max * structure.BoundsGridRatio;
            worldMax.y += structure.BoundsGridAddHeight;
            worldMax.y += worldMax.y * structure.BoundsGridExtraHeight;
            worldMax.x += worldMax.x * structure.BoundsGridExtraWidth;
            worldMax.z += worldMax.z * structure.BoundsGridExtraForward;
            worldMax.z += worldMax.z * structure.BoundsForward;
            worldMax.z += structure.BoundsGridShiftForward;

            // Convert world positions to grid indices
            Vector3Int gridMin = WorldToGridPosition(worldMin, cellSize);
            Vector3Int gridMax = WorldToGridPosition(worldMax, cellSize);

            // Calculate number of positions along each axis (inclusive)
            int countX = Mathf.Abs(gridMax.x - gridMin.x) + 1;
            int countY = Mathf.Abs(gridMax.y - gridMin.y) + 1;
            int countZ = Mathf.Abs(gridMax.z - gridMin.z) + 1;

            Grid3[] gridCells = new Grid3[countX * countY * countZ];
            int index = 0;

            for (int x = 0; x < countX; x++)
            {
                for (int y = 0; y < countY; y++)
                {
                    for (int z = 0; z < countZ; z++)
                    {
                        Grid3 cellIndex = new Grid3(
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

        // Convert a world position to grid index (rounded to nearest grid point)
        public static Vector3Int WorldToGridPosition(Vector3 worldPosition, float cellSize)
        {
            return new Vector3Int(
                Mathf.RoundToInt(worldPosition.x / cellSize),
                Mathf.RoundToInt(worldPosition.y / cellSize),
                Mathf.RoundToInt(worldPosition.z / cellSize)
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


        public virtual Bounds GetSmallGridBounds(Structure structure)
        {
            Bounds bounds = new Bounds(structure.Bounds.center, structure.Bounds.size);
            bounds.Expand(structure.BoundsExpand);
            Vector3 worldPosition1 = bounds.min * structure.BoundsGridRatio;
            worldPosition1.y += structure.BoundsGridAddBottom;
            worldPosition1.x += worldPosition1.x * structure.BoundsGridExtraWidth;
            worldPosition1.z += worldPosition1.z * structure.BoundsGridExtraForward;
            Vector3 worldPosition2 = bounds.max * structure.BoundsGridRatio;
            worldPosition2.y += structure.BoundsGridAddHeight;
            worldPosition2.y += worldPosition2.y * structure.BoundsGridExtraHeight;
            worldPosition2.x += worldPosition2.x * structure.BoundsGridExtraWidth;
            worldPosition2.z += (float)((double)worldPosition2.z * (double)structure.BoundsGridExtraForward + (double)worldPosition2.z * (double)structure.BoundsForward) + structure.BoundsGridShiftForward;
            bounds.min = ExtensionMethods.ToGrid(worldPosition1, SmallGrid.SmallGridSize, SmallGrid.SmallGridOffset).ToVector3();
            bounds.min -= structure.GridSize * 0.5f * Vector3.one;
            bounds.max = ExtensionMethods.ToGrid(worldPosition2, SmallGrid.SmallGridSize, SmallGrid.SmallGridOffset).ToVector3();
            bounds.max += structure.GridSize * 0.5f * Vector3.one;
            return bounds;
        }

    }
}
