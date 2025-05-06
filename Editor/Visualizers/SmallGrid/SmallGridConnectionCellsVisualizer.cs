using Assets.Scripts.Objects;
using Assets.Scripts.Util;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.visualizers
{

    /// <summary>
    /// Highlights the SmallGrid Cells occupied by the OpenEnd connections
    /// of a SmallGrid asset.
    /// </summary>
    public class SmallGridConnectionsVisualizer : IThingVisualizer
    {
        public void OnSceneGUI(SceneView sceneView, Object target)
        {
            if (!EditorPrefs.GetBool("Visualizer.Endpoints", true))
                return;

            // Only SmallGrids have OpenEnds
            SmallGrid smallGrid = target as SmallGrid;
            if (smallGrid == null)
                return;

            Handles.color = EditorPrefsHelper.LoadColor("Visualizers.OpenEnds.CellColor", new Color(1.0f, 0.5f, 0.0f, 1.0f));

            foreach (var openEnd in smallGrid.OpenEnds)
            {
                if (openEnd == null || openEnd.Transform == null)
                    continue;

                Vector3 snappedPos = SnapToGrid(openEnd.Transform.position, 0.5f, 0.25f);
                Handles.DrawWireCube(snappedPos, Vector3.one * 0.5f);
            }
        }

        /// <summary>
        /// Snap a world position to Grid metrics. 
        /// TODO: This function is not using offset
        /// TODO: Move this function to a helper
        /// </summary>
        /// <param name="position"></param>
        /// <param name="gridSize"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        Vector3 SnapToGrid(Vector3 position, float gridSize, float offset)
        {
            return new Vector3(
                Mathf.Round(position.x / gridSize) * gridSize,
                Mathf.Round(position.y / gridSize) * gridSize,
                Mathf.Round(position.z / gridSize) * gridSize
            );

        }
    }
}