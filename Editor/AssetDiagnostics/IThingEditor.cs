using UnityEngine;

namespace ilodev.stationeersmods.tools.diagnostics
{
    public enum EditorType
    {
        Before,
        After,
        Both
    }

    /// <summary>
    /// Interface for all types of custom editors
    /// </summary>
    public interface IThingEditor
    {
        EditorType Type { get; }
        void OnEnable(Object target);
        void OnDisable(Object target);
        int OnUpdate(Object target);
        int OnInspectorGUI(Object target, int defaultHidden);
    }

}
