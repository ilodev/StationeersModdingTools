using Assets.Scripts.Objects;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.visualizers
{

    public class SmallGridEndPointsVisualizer : IThingVisualizer
    {
        public void OnSceneGUI(SceneView sceneView, Object target)
        {
            SmallGrid smallGrid = target as SmallGrid;
            if (smallGrid == null)
                return;

            foreach (var openEnd in smallGrid.OpenEnds)
            {
                if (openEnd == null || openEnd.Transform == null)
                    continue;

                // Set color based on endpoint type
                Color color = Color.yellow;
                Handles.color = color;
                Handles.SphereHandleCap(0, openEnd.Transform.position, Quaternion.identity, 0.1f, EventType.Repaint);

                // Draw label
                Handles.color = Color.white;
                Handles.Label(openEnd.Transform.position + Vector3.up * 0.1f, openEnd.ConnectionType.ToString());
            }
        }
    }
}
