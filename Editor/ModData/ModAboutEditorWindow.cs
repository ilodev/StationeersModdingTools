using UnityEngine;
using UnityEditor;
using System.IO;

namespace ilodev.stationeersmods.tools.moddata
{
    public class ModAboutEditorWindow : EditorWindow
    {
        private const string AboutFolderPath = "Assets/About";
        private const string AboutXmlPath = "Assets/About/About.xml";
        private const string PreviewImagePath = "Assets/About/Preview.png";
        private const string ThumbnailImagePath = "Assets/About/Thumb.png";

        private ModAbout about;
        private Texture2D previewTexture;
        private bool generateThumbnail = false;
        private int previewPickerControlID;
        private Texture2D selectedPreviewTexture;

        [MenuItem("Modding/Open About")]
        public static void OpenWindow()
        {
            var window = GetWindow<ModAboutEditorWindow>("Mod About");
            window.LoadOrCreate();
            window.Show();
        }

        private void LoadOrCreate()
        {
            if (!Directory.Exists(AboutFolderPath))
                Directory.CreateDirectory(AboutFolderPath);

            if (File.Exists(AboutXmlPath))
            {
                string dummyPath = Path.Combine("Assets", "gamedata", "dummy.txt");
                about = ModAbout.Load(dummyPath);
            }

            if (about == null)
            {
                about = new ModAbout();
                about.Save(AboutXmlPath);
            }

            LoadPreviewTexture();
        }

        private void LoadPreviewTexture()
        {
            previewTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(PreviewImagePath);
        }

        private void OnGUI()
        {
            if (about == null)
            {
                EditorGUILayout.LabelField("No ModAbout loaded.");
                if (GUILayout.Button("Reload"))
                    LoadOrCreate();
                return;
            }

            EditorGUI.BeginChangeCheck();

            // Define Preview Width
            float targetWidth = 400f;
            float targetHeight = 0;

            // Make sure the window size adjusts based on the preview image's aspect ratio
            if (previewTexture != null)
            {
                // Maintain aspect ratio based on the current Preview image size
                float aspect = (float)previewTexture.height / previewTexture.width;
                targetHeight = targetWidth * aspect;

                // Update the window size based on the preview image's dimensions
                float windowWidth = targetWidth + 40f;  // 40 for padding around edges
                float windowHeight = targetHeight + 20f;  // Adjust the height to ensure UI elements are visible

                // Adjust window size dynamically but do not force a minimum
                this.minSize = new Vector2(windowWidth, windowHeight);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Metadata", EditorStyles.boldLabel);
            about.Name = EditorGUILayout.TextField("Name", about.Name);
            about.Author = EditorGUILayout.TextField("Author", about.Author);
            about.Version = EditorGUILayout.TextField("Version", about.Version);
            about.WorkshopHandle = (ulong)EditorGUILayout.LongField("Workshop Handle", (long)about.WorkshopHandle);

            EditorGUILayout.LabelField("Description");
            about.Description = EditorGUILayout.TextArea(about.Description, GUILayout.MinHeight(80));

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Tags", EditorStyles.boldLabel);
            if (about.Tags == null)
                about.Tags = new System.Collections.Generic.List<string>();

            int removeIndex = -1;
            for (int i = 0; i < about.Tags.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                about.Tags[i] = EditorGUILayout.TextField(about.Tags[i]);
                if (GUILayout.Button("X", GUILayout.Width(20)))
                    removeIndex = i;
                EditorGUILayout.EndHorizontal();
            }
            if (removeIndex >= 0)
                about.Tags.RemoveAt(removeIndex);

            if (GUILayout.Button("Add Tag"))
                about.Tags.Add("");

            EditorGUILayout.Space(10);

            // Custom preview display for the ObjectField
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Preview Image", EditorStyles.boldLabel);

            // If a texture is assigned, show it
            if (previewTexture != null)
            {
                float aspect = (float)previewTexture.height / previewTexture.width;
                float previewWidth = 512f;
                float previewHeight = previewWidth * aspect;

                // Draw the preview image
                Rect previewRect = GUILayoutUtility.GetRect(previewWidth, previewHeight, GUILayout.ExpandWidth(false));
                EditorGUI.DrawPreviewTexture(previewRect, previewTexture);

                // Detect clicks on the preview to open the picker
                if (Event.current.type == EventType.MouseDown && previewRect.Contains(Event.current.mousePosition))
                {
                    previewPickerControlID = GUIUtility.GetControlID(FocusType.Passive);
                    EditorGUIUtility.ShowObjectPicker<Texture2D>(previewTexture, false, "", previewPickerControlID);
                    Event.current.Use();
                }
            }
            else
            {
                // If no texture, show a button to pick one
                if (GUILayout.Button("Select Preview.png", GUILayout.Width(200), GUILayout.Height(40)))
                {
                    previewPickerControlID = GUIUtility.GetControlID(FocusType.Passive);
                    EditorGUIUtility.ShowObjectPicker<Texture2D>(null, false, "", previewPickerControlID);
                }
            }

            // Handle object picker result
            if (Event.current.commandName == "ObjectSelectorUpdated")
            {
                if (EditorGUIUtility.GetObjectPickerControlID() == previewPickerControlID)
                {
                    previewTexture = (Texture2D)EditorGUIUtility.GetObjectPickerObject();
                    GUI.changed = true;
                }
            }

            bool changedPreview = EditorGUI.EndChangeCheck();

            // Checkbox to toggle thumbnail generation
            generateThumbnail = EditorGUILayout.Toggle("Generate Thumbnail", generateThumbnail);

            EditorGUILayout.Space(10);

            if (EditorGUI.EndChangeCheck() || changedPreview)
            {
                about.Save(AboutXmlPath);
                Debug.Log("ModAbout updated and saved.");

                if (previewTexture != null)
                {
                    string path = AssetDatabase.GetAssetPath(previewTexture);
                    if (path != PreviewImagePath)
                    {
                        File.Copy(path, PreviewImagePath, true);
                        AssetDatabase.Refresh();
                    }

                    if (generateThumbnail)
                    {
                        GenerateThumbnailFromPreview();
                        AssetDatabase.Refresh();
                    }
                }
            }
        }


        private void GenerateThumbnailFromPreview()
        {
            if (previewTexture == null)
            {
                Debug.LogWarning("No preview texture found to generate thumbnail.");
                return;
            }

            // Load texture data from file
            string path = AssetDatabase.GetAssetPath(previewTexture);
            byte[] data = File.ReadAllBytes(path);
            Texture2D sourceTex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            sourceTex.LoadImage(data);

            int width = 863;
            int height = 495;

            Texture2D thumbnail = new Texture2D(width, height, TextureFormat.RGBA32, false);
            RenderTexture rt = RenderTexture.GetTemporary(width, height);
            Graphics.Blit(sourceTex, rt);

            RenderTexture.active = rt;
            thumbnail.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            thumbnail.Apply();

            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);

            byte[] pngData = thumbnail.EncodeToPNG();
            if (pngData != null)
            {
                File.WriteAllBytes(ThumbnailImagePath, pngData);
                Debug.Log($"Generated Thumbnail.png at {ThumbnailImagePath}");
                AssetDatabase.Refresh();
            }

            DestroyImmediate(thumbnail);
        }
    }

}
