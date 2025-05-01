using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.visualizers
{
    public class ThingInteratablesVisualizer : IThingVisualizer
    {
        public void OnSceneGUI(SceneView sceneView, Object target)
        {
            if (!EditorPrefs.GetBool("Visualizer.Interactables", true))
                return;

            Thing thing = target as Thing;
            if (thing == null)
                return;

            foreach (Interactable interactable in thing.Interactables)
            {
                Handles.color = new Color(1.0f, 0.5f, 0.9f, 1.0f); // Purple
                Transform slotTransform = GetSlotTransform(thing, interactable.Action);
                Vector3 position = interactable.Bounds.center + interactable.Parent.transform.position + slotTransform.position;
                Handles.DrawWireCube(position, interactable.Bounds.size);

                // Draw label
                Handles.color = Color.white;
                GUIStyle boldLabel = new GUIStyle(EditorStyles.label);
                boldLabel.richText = true;
                string text = $"<color=#FFFFFF><b>{interactable.DisplayName.ToString()}</b></color>\r\n{interactable.Action.ToString()}";
                Handles.Label(interactable.Parent.transform.position + Vector3.up * 0.1f, text, boldLabel);
            }
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
