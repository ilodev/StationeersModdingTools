using System.Collections;
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ilodev.stationeersmods.tools.visualizers
{
    [Overlay(typeof(SceneView), "Visualizers", true)]
    public class ThingVisualizerOverlay : Overlay
    {
        public override VisualElement CreatePanelContent()
        {
            var root = new VisualElement();

            // Add toggles for each visualizer have
            root.Add(CreateToggle("End Points", "Visualizer.Endpoints", true));
            root.Add(CreateToggle("Grid Bounds", "Visualizer.GridBounds", true));
            root.Add(CreateToggle("Large Grid Bounds", "Visualizer.LargeGridBounds", true));
            root.Add(CreateToggle("Small Grid Bounds", "Visualizer.SmallGridBounds", true));
            root.Add(CreateToggle("Interactables", "Visualizer.Interactables", true));

            return root;
        }

        VisualElement CreateToggle(string label, string key, bool defaultValue)
        {
            var toggle = new Toggle(label)
            {
                value = EditorPrefs.GetBool(key, defaultValue)
            };

            toggle.RegisterValueChangedCallback(evt =>
            {
                EditorPrefs.SetBool(key, evt.newValue);
                SceneView.RepaintAll();
            });

            return toggle;
        }


    }
}
