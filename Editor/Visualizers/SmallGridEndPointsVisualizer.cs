using Assets.Scripts.Objects;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ilodev.stationeersmods.tools.visualizers
{

    public class SmallGridEndPointsVisualizer : IThingVisualizer
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

                Color color = Color.green;

                switch (openEnd.ConnectionType )
                {
                    case NetworkType.Pipe:
                        color = Color.yellow;
                        break;

                    case NetworkType.Power:
                    case NetworkType.Data:
                    case NetworkType.PowerAndData:
                        color = Color.red;
                        break;

                    default:
                        color = Color.green;
                        break;
                }


                // Set color based on endpoint type
                Handles.color = color;

                Handles.SphereHandleCap(0, openEnd.Transform.position, Quaternion.identity, 0.1f, EventType.Repaint);

                Handles.ArrowHandleCap(
                    0, 
                    openEnd.Transform.position, // position at which to draw the arrow
                    Quaternion.LookRotation(openEnd.Transform.forward), // rotation for the arrow
                    0.1f, // size of the arrow
                    EventType.Repaint // always Repaint for scene GUI
                );

                // Draw label
                Handles.color = Color.white;
                GUIStyle boldLabel = new GUIStyle(EditorStyles.label);
                boldLabel.richText = true;
                string text = $"<color=#FFFFFF><b>{openEnd.ConnectionType.ToString()}</b></color>\r\n{openEnd.ConnectionRole.ToString()}";
                Handles.Label(openEnd.Transform.position + Vector3.up * 0.1f, text, boldLabel);
            }
        }
    }
}
