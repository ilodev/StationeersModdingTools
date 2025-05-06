using Assets.Scripts.GridSystem;
using Assets.Scripts.Objects;
using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.visualizers
{
    public class StructureForceGridBoundsVisualizer : IThingVisualizer
    {
        public void OnSceneGUI(SceneView sceneView, Object target)
        {

            if (!EditorPrefs.GetBool("Visualizer.ForceGridBounds", true))
                return;

            Structure structure = target as Structure;
            if (structure == null)
                return;

            foreach(Grid3 gridCell in structure.ForceGridBounds)
            {
                Handles.color = new Color(1f, 1f, 1f, 0.3f); // cyan, semi-transparent
                Handles.DrawWireCube(gridCell.ToVector3(), Vector3.one * SmallGrid.SmallGridSize);
            }
        }
    }
}
