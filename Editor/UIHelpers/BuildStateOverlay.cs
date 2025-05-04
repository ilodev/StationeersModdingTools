using Assets.Scripts.Objects;
using Assets.Scripts.Util;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[Overlay(typeof(SceneView), "BuildState", true)]
public class BuildStateOverlay : Overlay
{
    // Sample options for the dropdown
    private string[] buildStates = new string[] { "Idle", "Building", "Completed", "Failed" };
    private PopupField<string> dropdown;

    public BuildStateOverlay()
    {
        // Subscribe to the selection change event
        Selection.selectionChanged += OnSelectionChanged;
    }

    // Unsubscribe to prevent memory leaks when the overlay is disposed
    ~BuildStateOverlay()
    {
        Selection.selectionChanged -= OnSelectionChanged;
    }

    private void OnSelectionChanged()
    {
        // Check if we have a valid selection in the hierarchy
        var selectedObject = Selection.activeGameObject;

        if (selectedObject != null)
        {
            Debug.Log($"Selected Object: {selectedObject.name}");
            Structure structure = selectedObject.GetComponent<Structure>();

            // Dynamically update dropdown options based on selected object
            var newStates = GetStatesForSelectedObject(selectedObject);
            if (dropdown != null)
            { 
                // Update dropdown options using the 'choices' property
                dropdown.choices = new List<string>(newStates);  // Update the options

                // Optionally set the default value (first item)
                dropdown.SetValueWithoutNotify(newStates[0]);
            }
        }
    }

    // This function can be customized to fetch relevant states based on the selected object
    private string[] GetStatesForSelectedObject(GameObject selectedObject)
    {
        // Example: Replace with your logic to get build states for the selected object
        if (selectedObject.name.Contains("Building"))
        {
            return new string[] { "Idle", "Building", "In Progress" };
        }
        else
        {
            return buildStates;  // Default states
        }
    }

    public override VisualElement CreatePanelContent()
    {
        var root = new VisualElement();

        // Label
        var label = new UnityEngine.UIElements.Label("Select Build State:");
        label.style.marginBottom = 5;
        root.Add(label);

        var dropdown = new PopupField<string>(null, buildStates.ToList<string>(), 0, null, null);

        // Optional: Handle change events if needed
        dropdown.RegisterValueChangedCallback(evt =>
        {
            Debug.Log($"Selected state: {evt.newValue}");
        });

        // Add dropdown to the root container
        root.Add(dropdown);

        return root;
    }
}
    