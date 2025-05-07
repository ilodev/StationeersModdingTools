using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Items;
using ilodev.stationeersmods.tools.assetsfactory;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.contextualtools
{
    public class ContextMenuStackableSplit : Editor
    {
        [MenuItem("GameObject/Interactable/Stackable/Split One", false, 10)]
        static void AddInteractableSplitOne(MenuCommand menuCommand)
        {
            // Get the selected object
            GameObject selectedObject = menuCommand.context as GameObject;

            if (selectedObject == null || !ContextMenuValidator.Validate<Stackable>(selectedObject))
                return;

            Thing thing = selectedObject.GetComponent<Thing>();
            InteractableHelpers.AddInteractable(thing, "SplitOne", InteractableType.Button1);
        }

        [MenuItem("GameObject/Interactable/Stackable/Split One", true)]
        static bool AddInteractableSplitOne()
        {
            // Use the shared validation function
            return ContextMenuValidator.Validate<Stackable>(Selection.activeGameObject);
        }

        [MenuItem("GameObject/Interactable/Stackable/Split Half", false, 10)]
        static void AddInteractableSplitHalf(MenuCommand menuCommand)
        {
            // Get the selected object
            GameObject selectedObject = menuCommand.context as GameObject;

            if (selectedObject == null || !ContextMenuValidator.Validate<Stackable>(selectedObject))
                return;

            Thing thing = selectedObject.GetComponent<Thing>();
            InteractableHelpers.AddInteractable(thing, "SplitHalf", InteractableType.Button2);
        }

        [MenuItem("GameObject/Interactable/Stackable/Split Half", true)]
        static bool AddInteractableSplitHalf()
        {
            // Use the shared validation function
            return ContextMenuValidator.Validate<Stackable>(Selection.activeGameObject);
        }

    }
}
