using Assets.Scripts.Objects;
using Assets.Scripts.Util;
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

            // Draw a cyan box around the object Bounds
            Handles.color = new Color(0f, 1f, 1f, 0.3f); // cyan, semi-transparent
            Handles.DrawWireCube(center, size);

            // Draw a white cube around the smallgrid cells blocked
            Bounds test = GetSmallGridBounds(structure);
            Handles.color = new Color(1f, 1f, 1f, 1.0f); // cyan, semi-transparent
            Handles.DrawWireCube(test.center, test.size);

        }



        /// <summary>
        ///  This gives a collider box based on the grid size of the structure
        /// </summary>
        /// <param name="structure"></param>
        /// <returns></returns>
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
