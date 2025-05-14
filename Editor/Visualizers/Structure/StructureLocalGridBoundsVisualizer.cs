using Assets.Scripts.GridSystem;
using Assets.Scripts.Objects;
using Assets.Scripts.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeers.moddingtools.visualizers
{
    public class StructureLocalGridBoundsVisualizer : IThingVisualizer
    {
        public void OnSceneGUI(SceneView sceneView, UnityEngine.Object target)
        {

            if (!EditorPrefs.GetBool("Visualizer.ForceGridBounds", true))
                return;

            Structure structure = target as Structure;
            if (structure == null)
                return;

            Grid3[] localGridBounds = GetLocalGridBounds(structure);
            Debug.Log($"LocalGridBounds {localGridBounds.Length}");

            foreach (Grid3 gridCell in localGridBounds)
            {
                Color face = new Color(1f, 1f, 1f, 0.05f); // cyan, semi-transparent
                Color line = Color.red;
                DrawingUtils.DrawSolidCube(gridCell.ToVector3(), structure.GridSize, face, line);
            }
        }

        public Grid3[] GetLocalGridBounds(Structure structure)
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
    }
}