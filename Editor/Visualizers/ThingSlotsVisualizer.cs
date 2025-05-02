using System;
using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Drawing;

namespace ilodev.stationeersmods.tools.visualizers
{
    public class ThingSlotsVisualizer : IThingVisualizer
    {
        public void OnSceneGUI(SceneView sceneView, UnityEngine.Object target)
        {
            if (!EditorPrefs.GetBool("Visualizer.Slots", true))
                return;

            Thing thing = target as Thing;
            if (thing == null)
                return;

            foreach (Slot slot in thing.Slots)
            {
                Handles.color = new UnityEngine.Color(0.8f, 0.8f, 0.3f, 1.0f); // Purple
                Vector3 position = Vector3.zero;
                
                if (slot.Location != null)
                    position = slot.Location.position;

                Vector3 size = slot.Size;

                // override size with collider size
                if (slot.Collider != null && size == Vector3.zero)
                {
                    size = slot.Collider.bounds.size;
                }

                // No size at the end, no need to display
                if (size == Vector3.zero)
                    continue;

                if (slot.Collider != null)
                {
                    position += slot.Collider.center;
                }

                if (slot.Location != null)
                {
                    WithHandlesMatrix(Matrix4x4.TRS(position, slot.Location.rotation, Vector3.one), () =>
                    {
                        Handles.DrawWireCube(Vector3.zero, size);
                    });
                }
                else
                {
                    Handles.DrawWireCube(position, size);
                }

                // Draw label
                GUIStyle boldLabel = new GUIStyle(EditorStyles.label);
                boldLabel.richText = true;
                string text = $"<color=#FFFFFF><b>{slot.StringKey.ToString()} ({slot.Type.ToString()})</b></color>\r\n<color=#000000>{slot.Action.ToString()}</color>";
                Handles.color = UnityEngine.Color.white;
                Handles.Label(position, text, boldLabel);
            }
        }

        void WithHandlesMatrix(Matrix4x4 matrix, Action drawAction)
        {
            var oldMatrix = Handles.matrix;
            Handles.matrix = matrix;
            drawAction();
            Handles.matrix = oldMatrix;
        }
    }
}
