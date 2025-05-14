using Assets.Scripts.Objects;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ilodev.stationeers.moddingtools.visualizers
{
    /// <summary>
    /// Class to visualize OpenEnd connections
    /// </summary>
    public class SmallGridEndPointsVisualizer : IThingVisualizer
    {
        public void OnSceneGUI(SceneView sceneView, Object target)
        {
            if (!EditorPrefs.GetBool("Visualizer.OpenEnds", true))
                return;

            // Only SmalGrid structures have OpenEnds
            SmallGrid smallGrid = target as SmallGrid;
            if (smallGrid == null)
                return;

            foreach (var openEnd in smallGrid.OpenEnds)
            {
                // If there is transform, then it is probably a misconfigured OpenEnd
                if (openEnd == null || openEnd.Transform == null)
                    continue;

                // Set color based on endpoint type
                Color color;
                switch (openEnd.ConnectionType )
                {
                    case NetworkType.Pipe:
                        color = Color.yellow;
                        break;

                    case NetworkType.PipeLiquid:
                        color = Color.blue;
                        break;

                    case NetworkType.Chute:
                        color = Color.gray;
                        break;

                    case NetworkType.Power:
                    case NetworkType.Data:
                    case NetworkType.PowerAndData:
                        color = Color.red;
                        break;

                    // Any complex combination, or by default use Green color.
                    default:
                        color = Color.green;
                        break;
                }
                Handles.color = new Color(color.r, color.g, color.b, 0.6f);

                // TODO: Move all this drawing to the drawing util

                // Make a small colored sphere
                Handles.SphereHandleCap(0, openEnd.Transform.position, Quaternion.identity, 0.1f, EventType.Repaint);

                // Draw a small colord arrow
                Handles.ArrowHandleCap(
                    0, 
                    openEnd.Transform.position - openEnd.Transform.forward * 0.1f, // position at which to draw the arrow
                    Quaternion.LookRotation(openEnd.Transform.forward), // rotation for the arrow
                    0.2f, // size of the arrow
                    EventType.Repaint // always Repaint for scene GUI
                );

                // Draw a label
                Handles.color = Color.white;
                GUIStyle boldLabel = new GUIStyle(EditorStyles.label);
                boldLabel.richText = true;
                string text = $"<color=#FFFFFF><b>{openEnd.ConnectionType.ToString()}</b></color>\r\n{openEnd.ConnectionRole.ToString()}";
                Handles.Label(openEnd.Transform.position + Vector3.up * 0.1f, text, boldLabel);
            }
        }
    }
}
