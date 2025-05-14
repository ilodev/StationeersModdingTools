using UnityEditor;
using UnityEngine;

namespace ilodev.stationeers.moddingtools.visualizers
{ 
    /// <summary>
    /// Interface for all types of visual editors
    /// </summary>
    public interface IThingVisualizer
    {
        /// <summary>
        /// OnSceneGUI will be called when a scene (or prefab view) is being rendered.
        /// </summary>
        /// <param name="sceneView"></param>
        /// <param name="target"></param>
        void OnSceneGUI(SceneView sceneView, Object target);
    }

}