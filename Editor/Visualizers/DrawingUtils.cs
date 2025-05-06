using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.visualizers
{
    public static class DrawingUtils 
    {

        public static void DrawSolidCube(Vector3 center, float size, Color faceColor, Color outlineColor)
        {

            Vector3 halfSize = Vector3.one * (size * 0.5f);

            // Define the 8 corners of the cube
            Vector3[] corners = new Vector3[8];
            corners[0] = center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z);
            corners[1] = center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z);
            corners[2] = center + new Vector3(halfSize.x, -halfSize.y, halfSize.z);
            corners[3] = center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z);
            corners[4] = center + new Vector3(-halfSize.x, halfSize.y, -halfSize.z);
            corners[5] = center + new Vector3(halfSize.x, halfSize.y, -halfSize.z);
            corners[6] = center + new Vector3(halfSize.x, halfSize.y, halfSize.z);
            corners[7] = center + new Vector3(-halfSize.x, halfSize.y, halfSize.z);

            // Define each face by its 4 corners (order matters for proper winding)
            int[][] faces = new int[][]
            {
            new int[]{0, 1, 2, 3}, // Bottom
            new int[]{7, 6, 5, 4}, // Top
            new int[]{4, 5, 1, 0}, // Front
            new int[]{6, 7, 3, 2}, // Back
            new int[]{5, 6, 2, 1}, // Right
            new int[]{7, 4, 0, 3}, // Left
            };

            // Draw each face
            foreach (var face in faces)
            {
                Handles.DrawSolidRectangleWithOutline(
                    new Vector3[]
                    {
                    corners[face[0]],
                    corners[face[1]],
                    corners[face[2]],
                    corners[face[3]]
                    },
                    faceColor,
                    outlineColor
                );
            }
        }


    }
}
