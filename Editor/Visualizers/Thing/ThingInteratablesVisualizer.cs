using System;
using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Drawing;

namespace ilodev.stationeersmods.tools.visualizers
{
    public class ThingInteratablesVisualizer : IThingVisualizer
    {
        public void OnSceneGUI(SceneView sceneView, UnityEngine.Object target)
        {
            if (!EditorPrefs.GetBool("Visualizer.Interactables", true))
                return;

            Thing thing = target as Thing;
            if (thing == null)
                return;

            foreach (Interactable interactable in thing.Interactables)
            {
                Handles.color = new UnityEngine.Color(1.0f, 0.5f, 0.9f, 1.0f); // Purple


                Vector3 position = interactable.Bounds.center;/// + interactable.Parent.transform.position;
                Vector3 size = interactable.Bounds.size;

                // override size with collider size
                if (interactable.Collider != null && size == Vector3.zero)
                {
                    size = interactable.Collider.bounds.size;
                }

                // No size at the end, no need to display
                if (size == Vector3.zero)
                    continue;

                if (interactable.Bounds.size == Vector3.zero)
                    size = new Vector3(0.01f, 0.01f, 0.01f);

                Transform slotTransform = GetSlotTransform(thing, interactable.Action);

                if (interactable.Collider != null)
                {
                    position = interactable.Collider.bounds.center;
                }
                else
                {
                    if (slotTransform != null) position += slotTransform.position;
                }
                
                if (interactable.Collider != null)
                {
                    WithHandlesMatrix(Matrix4x4.TRS(position, interactable.Collider.transform.rotation, Vector3.one), () =>
                    {
                        Handles.DrawWireCube(Vector3.zero, size);
                    });
                }
                else if (slotTransform != null)
                {
                    WithHandlesMatrix(Matrix4x4.TRS(position, slotTransform.rotation, Vector3.one), () =>
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
                string text = $"<color=#FFFFFF><b>{interactable.StringKey.ToString()}</b></color>\r\n{interactable.Action.ToString()}";
                Handles.color = UnityEngine.Color.white;
                Handles.Label(position, text, boldLabel);
            }
        }

        /// <summary>
        /// Draw a square rotated
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="drawAction"></param>
        void WithHandlesMatrix(Matrix4x4 matrix, Action drawAction)
        {
            var oldMatrix = Handles.matrix;
            Handles.matrix = matrix;
            drawAction();
            Handles.matrix = oldMatrix;
        }

        Transform GetSlotTransform(Thing thing, InteractableType interactableType)
        {
            foreach (Slot slot in thing.Slots)
            {
                if (slot.Action == interactableType)
                {
                    return slot.Location;
                }
            }
            return (default(Transform));
        }

    }
}
