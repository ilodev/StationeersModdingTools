using Assets.Scripts.GridSystem;
using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeers.moddingtools.visualizers
{
    public class StructureForceGridBoundsVisualizer : IThingVisualizer
    {
        public void OnSceneGUI(SceneView sceneView, Object target)
        {

            if (!EditorPrefs.GetBool("Visualizer.ForceGridBounds", true))
                return;

            // SmallGrid structures ignore the force grid blocking.
            if (target as SmallGrid != null)
                return;

            Structure structure = target as Structure;
            if (structure == null)
                return;

            foreach(Grid3 gridCell in structure.ForceGridBounds)
            {
                Color face = new Color(1f, 1f, 1f, 0.05f); // cyan, semi-transparent
                Color line = Color.white;
                DrawingUtils.DrawSolidCube(gridCell.ToVector3(), structure.GridSize, face, line);
            }
        }
    }
}
