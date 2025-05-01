using Assets.Scripts.Objects;
using Assets.Scripts.Util;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.visualizers
{

    public class SmallGridConnectionsVisualizer : IThingVisualizer
    {
        public void OnSceneGUI(SceneView sceneView, Object target)
        {
            if (!EditorPrefs.GetBool("Visualizer.Endpoints", true))
                return;

            SmallGrid smallGrid = target as SmallGrid;
            if (smallGrid == null)
                return;

            foreach (var openEnd in smallGrid.OpenEnds)
            {
                if (openEnd == null || openEnd.Transform == null)
                    continue;

                Handles.color = new Color(1.0f, 0.5f, 0.0f, 1.0f); // Orange
                Vector3 snappedPos = SnapToGrid(openEnd.Transform.position, 0.5f, 0.25f);
                Handles.DrawWireCube(snappedPos, Vector3.one * 0.5f);
            }
        }

        Vector3 SnapToGrid(Vector3 position, float gridSize, float offset)
        {
            /*
            return new Vector3(
                Mathf.Floor((position.x - offset) / gridSize) * gridSize + offset,
                Mathf.Floor((position.y - offset) / gridSize) * gridSize + offset,
                Mathf.Floor((position.z - offset) / gridSize) * gridSize + offset
            );
            */
            return new Vector3(
                Mathf.Round(position.x / gridSize) * gridSize,
                Mathf.Round(position.y / gridSize) * gridSize,
                Mathf.Round(position.z / gridSize) * gridSize
            );

        }
    }
}