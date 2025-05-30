using Assets.Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeers.moddingtools.uihelpers
{

    [CustomEditor(typeof(Wireframe), true)]
    public class WireFrameInspector : Editor
    {
        /// <summary>
        /// Should the edges be drawn in the viewport
        /// </summary>
        private bool ShowEdges = false;

        /// <summary>
        /// Current inspector target
        /// </summary>
        private Wireframe wireframe;

        /// <summary>
        /// Reference source transform to create the blueprint mesh from.
        /// </summary>
        private Transform BlueprintSource;

        private void Awake()
        {
            wireframe = (Wireframe)target;
        }

        /// <summary>
        /// Build the inspector/editor GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(10);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Modding tools", EditorStyles.boldLabel);
            GUILayout.Space(10);

            /// Try wiring Transform, MeshFilter and MeshRenderer from the current gameobject 
            AutoAssignFromCurrentGameObject(wireframe);

            /// Logic to enable show the save mesh button
            bool saveMeshEnabled = (wireframe.BlueprintMeshFilter != null && wireframe.BlueprintMeshFilter.sharedMesh != null && wireframe.BlueprintMeshFilter.sharedMesh.name == "");
            GUI.enabled = saveMeshEnabled;
            if (GUILayout.Button("Save Mesh"))
            {
                Debug.Log("Saving mesh");
                SaveMeshFromObject(wireframe, "Meshes", target.name);
            }
            GUI.enabled = true;

            /// Logic to show Generate edges from current mesh.
            bool GenEdgesEnabled = wireframe.BlueprintMeshFilter != null && wireframe.BlueprintMeshFilter.sharedMesh != null;
            GUI.enabled = GenEdgesEnabled;
            if (GUILayout.Button("Generate Edges from current mesh"))
            {
                GenerateWireframeEdges(wireframe, wireframe.BlueprintTransform);
                // Make dirty to force saving
                EditorUtility.SetDirty(target);
                Debug.Log("Generating Edges finished");
            }
            GUI.enabled = true;
            GUILayout.Space(10);

            BlueprintSource = (Transform)EditorGUILayout.ObjectField(
                new GUIContent("Source GameObject", "This is the object the tool will use to build the prefab mesh."),
                BlueprintSource, 
                typeof(Transform), 
                true
            );
            /// Logic to show Generate edges from source gameobject.
            bool SourceEnabled = BlueprintSource != null;
            GUI.enabled = SourceEnabled;
            if (GUILayout.Button("Generate Edges and Mesh from Source"))
            {
                GenerateWireframeEdgesAndCombinedMesh(wireframe, BlueprintSource);
                // Make dirty to force saving
                EditorUtility.SetDirty(target);
                Debug.Log("Generating Edges/Mesh finished");
            }
            GUI.enabled = true;
            GUILayout.Space(10);

            /// Logic to show Edges toggle.
            bool ShowEdgesEnabled = (wireframe.WireframeEdges.Count > 0);
            GUI.enabled = ShowEdgesEnabled;
            ShowEdges = ShowEdgesEnabled ? ShowEdges : false;
            //ShowEdges = GUILayout.Toggle(ShowEdges, "Show Edges");
            ShowEdges = EditorGUILayout.Toggle("Show Edges", ShowEdges);
            GUI.enabled = true;

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Generates the blueprint Edges, CombinedMesh and Bounds
        /// </summary>
        /// <param name="wireframe"></param>
        /// <param name="target"></param>
        public static void GenerateWireframeEdgesAndCombinedMesh(Wireframe wireframe, Transform target)
        {
            WireframeGenerator generator = new WireframeGenerator(target);
            wireframe.WireframeEdges = generator.Edges;
            if (wireframe.BlueprintMeshFilter != null)
                wireframe.BlueprintMeshFilter.sharedMesh = generator.CombinedMesh;
            wireframe.BlueprintBounds = generator.CombinedMesh.bounds;
        }

        /// <summary>
        /// Only regenerate the blueprint Edges and bounds.
        /// </summary>
        /// <param name="wireframe"></param>
        /// <param name="target"></param>
        public static void GenerateWireframeEdges(Wireframe wireframe, Transform target)
        {
            WireframeGenerator generator = new WireframeGenerator(target);
            wireframe.WireframeEdges = generator.Edges;
            wireframe.BlueprintBounds = generator.CombinedMesh.bounds;
        }

        /// <summary>
        /// Auto assign Transform, MeshFilter and MeshRenderer from the current 
        /// GameObject the component is attached to.
        /// </summary>
        /// <param name="wireframe"></param>
        private void AutoAssignFromCurrentGameObject(Wireframe wireframe)
        {
            /// Autoassign property if present in the same game object
            if (wireframe.BlueprintTransform == null)
                wireframe.BlueprintTransform = wireframe.transform;

            /// Autoassign property if present in the same game object
            if (wireframe.BlueprintMeshFilter == null)
                wireframe.BlueprintMeshFilter = wireframe.GetComponent<MeshFilter>();

            /// Autoassign property if present in the same game object
            if (wireframe.BlueprintRenderer == null)
                wireframe.BlueprintRenderer = wireframe.GetComponent<MeshRenderer>();
            else
                if (wireframe.BlueprintRenderer.sharedMaterial == null)
                    wireframe.BlueprintRenderer.sharedMaterial = CreateBlueprintMaterial();  
        }

        /// <summary>
        /// Creates a ghost material that is only used for rendering the current view and not saved
        /// </summary>
        /// <returns></returns>
        private Material CreateBlueprintMaterial()
        {
            // Create a new material with the Standard shader
            Material transparentMat = new Material(Shader.Find("Standard"));
            transparentMat.name = "(Ghost) blueprint material";

            // Set color with alpha (RGBA)
            transparentMat.color = new Color(0f, 1f, 0f, 0.2f); // green, 20% transparent

            // Set rendering mode to Transparent
            transparentMat.SetFloat("_Mode", 3); // 3 = Transparent
            transparentMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            transparentMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            transparentMat.SetInt("_ZWrite", 0);
            transparentMat.DisableKeyword("_ALPHATEST_ON");
            transparentMat.EnableKeyword("_ALPHABLEND_ON");
            transparentMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            // Disable smoothness
            transparentMat.SetFloat("_Glossiness", 0f);
            // Enable emission keyword
            transparentMat.EnableKeyword("_EMISSION");
            transparentMat.SetColor("_EmissionColor", transparentMat.color * 1.0f);

            transparentMat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            return transparentMat;
        }

        /// <summary>
        /// Used to draw the edges on the prefab/scene view
        /// </summary>
        public void OnSceneGUI()
        {
            if (ShowEdges == true)
            {
                DrawWireFrameEdges(wireframe.WireframeEdges, new Color(0, 255, 0, 0.8f));
                SceneView.RepaintAll();
            }
        }

        /// <summary>
        /// Draw a list of edges using Editor Handles
        /// </summary>
        /// <param name="edges">List of edges to draw</param>
        /// <param name="color">Color of the line</param>
        public static void DrawWireFrameEdges(List<Edge> edges, Color color)
        {
            Color prevColor = Handles.color;
            Handles.color = color;

            foreach (var edge in edges)
                Handles.DrawLine(edge.Point1, edge.Point2, 0.02f);

            Handles.color = prevColor;
        }

        /// <summary>
        /// Saves a mesh 
        /// </summary>
        /// <param name="wireframe"></param>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="askForFolder"></param>
        public static void SaveMeshFromObject(Wireframe wireframe, string path, string name, bool askForFolder = true)
        {
            MeshFilter meshFilter = wireframe.BlueprintMeshFilter;
            if (meshFilter == null || meshFilter.sharedMesh == null)
            {
                Debug.LogWarning("No MeshFilter with a valid Mesh found on the selected object.");
                return;
            }

            Mesh meshToSave = meshFilter.sharedMesh;

            // Set a known starting path (inside Assets folder)
            string startingPath = Path.Combine(Application.dataPath, path);
            if (!Directory.Exists(startingPath))
                Directory.CreateDirectory(startingPath);

            string filepath;

            if (askForFolder)
            {
                // Open a save file panel with a default file name
                filepath = EditorUtility.SaveFilePanel(
                    "Save Mesh As",
                    startingPath,
                    name + ".asset",
                    "asset"
                );
            }
            else
                filepath = Path.Combine(startingPath, name + ".asset");

            if (string.IsNullOrEmpty(filepath))
                return;

            // Convert absolute path to relative project path
            string relativePath = GetRelativeAssetsPath(filepath);

            // Save the mesh as a new asset
            Mesh newMesh = Instantiate(meshToSave);
            AssetDatabase.CreateAsset(newMesh, relativePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            wireframe.BlueprintMeshFilter.sharedMesh = newMesh;
            Debug.Log("Mesh saved to: " + relativePath);
        }

        /// <summary>
        /// Returns the Assets/ relative path of a file
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <returns></returns>
        public static string GetRelativeAssetsPath(string absolutePath)
        {
            string assetsPath = Application.dataPath;
            if (absolutePath.StartsWith(assetsPath))
            {
                return "Assets" + absolutePath.Substring(assetsPath.Length);
            }
            else
            {
                Debug.LogWarning("Path is outside the Assets folder.");
                return null;
            }
        }

        private void OnDestroy()
        {
           // Debug.Log("stop watching");
        }
    }
}
