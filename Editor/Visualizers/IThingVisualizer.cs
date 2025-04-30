using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.visualizers
{ 
    /// <summary>
    /// Interface for all types of custom editors
    /// </summary>
    public interface IThingVisualizer
    {
        void OnSceneGUI(SceneView sceneView, Object target);
    }

}