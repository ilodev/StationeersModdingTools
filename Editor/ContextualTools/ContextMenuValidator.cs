using UnityEngine;

namespace ilodev.stationeersmods.tools.contextualtools
{
    /// <summary>
    /// General validation class for contextual menus
    /// </summary>
    public static class ContextMenuValidator
    {
        // Reusable validation function
        public static bool Validate<T>(GameObject selectedObject) where T : MonoBehaviour
        {
            if (selectedObject == null) return false;
            return selectedObject.GetComponent<T>() != null;
        }
    }
}

