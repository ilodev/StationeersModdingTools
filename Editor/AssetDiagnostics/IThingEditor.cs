using UnityEngine;

namespace ilodev.stationeers.moddingtools.diagnostics
{
    /// <summary>
    /// When an editor has to be called (before the original, after the original, never both)
    /// </summary>
    public enum EditorType
    {
        Before,
        After
    }

    /// <summary>
    /// Interface for all types of custom editors. Editors are called on Inspector Update
    /// and on Inspector GUI.
    /// </summary>
    public interface IThingEditor
    {
        /// <summary>
        /// Defines the editor type
        /// </summary>
        EditorType Type { get; }

        /// <summary>
        /// Called once to initialize the editor
        /// </summary>
        /// <param name="target"></param>
        void OnEnable(Object target);

        /// <summary>
        /// Called once to deinitialize the editor
        /// </summary>
        /// <param name="target"></param>
        void OnDisable(Object target);

        /// <summary>
        /// Something has been updated in the editor (not necessarily this game object)
        /// </summary>
        /// <param name="target"></param>
        /// <returns>boolean true if the object has to be mark as dirty because of a change</returns>
        int OnUpdate(Object target);

        /// <summary>
        /// Add information to the inspector GUI
        /// </summary>
        /// <param name="target"></param>
        /// <param name="defaultHidden">true if the original inspector has is hidden</param>
        /// <returns>boolean true if the original inspector needs to be hidden</returns>
        int OnInspectorGUI(Object target, int defaultHidden);
    }

}
