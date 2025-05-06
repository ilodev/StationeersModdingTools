using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;

namespace ilodev.stationeersmods.tools.visualizers
{
    /// <summary>
    /// Creates an overlay panel for the visualizers
    /// </summary>
    [Overlay(typeof(SceneView), "Visualizers", true)]
    public class ThingVisualizerOverlay : Overlay
    {
        /// <summary>
        /// Creates the main visualizers panel. Calls visualizer to add new toggle options
        /// </summary>
        /// <returns></returns>
        public override VisualElement CreatePanelContent()
        {
            var root = new VisualElement();

            // Add toggles for all the visualizer we have
            root.Add(CreateToggle("End Points", "Visualizer.Endpoints", true, "Visualize OpenEnd connection data: type, role, etc"));
            root.Add(CreateToggle("Grid Bounds", "Visualizer.GridBounds", true));
            root.Add(CreateToggle("Large Grid Bounds", "Visualizer.LargeGridBounds", true));
            root.Add(CreateToggle("Small Grid Bounds", "Visualizer.SmallGridBounds", true));
            root.Add(CreateToggle("Force Grid Bounds", "Visualizer.ForceGridBounds", true));
            root.Add(CreateToggle("Interactables", "Visualizer.Interactables", true, "Show the interactables data: name, type, collider, etc"));
            root.Add(CreateToggle("Slots", "Visualizer.Slots", true, "Highlight Slot information: Name, type, shape, etc"));

            // TODO: Move Toggle definition to the visualizer, and add additional toggles provided
            // by the visualizers here.
            foreach(var visualizer in ThingVisualizer.GetVisualizers())
            {
            }

            return root;
        }

        /// <summary>
        /// Creates a toggle.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <param name="tooltip"></param>
        /// <returns></returns>
        VisualElement CreateToggle(string label, string key, bool defaultValue = true, string tooltip = "")
        {
            var toggle = new Toggle(label)
            {
                value = EditorPrefs.GetBool(key, defaultValue)

            };
            toggle.tooltip = tooltip;

            toggle.RegisterValueChangedCallback( evt =>
            {
                EditorPrefs.SetBool(key, evt.newValue);
                SceneView.RepaintAll();
            });

            return toggle;
        }


    }
}
