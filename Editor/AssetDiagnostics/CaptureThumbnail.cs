using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ilodev.stationeersmods.tools.diagnostics
{
    public static class ThumbnailRenderer
    {
        public static void CaptureThumbnail(Assets.Scripts.Objects.Thing thing, int resolution = 512)
        {
            // Create a parent container to hide temp objects from hierarchy and prevent saving
            var container = new GameObject("ThumbnailCaptureContainer");
            container.hideFlags = HideFlags.HideAndDontSave;

            // Create camera as child of container
            var camGO = new GameObject("ThumbnailCamera");
            camGO.hideFlags = HideFlags.HideAndDontSave;
            camGO.transform.parent = container.transform;
            var cam = camGO.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0, 0, 0, 0);
            cam.cullingMask = ~0; // everything
            cam.orthographic = false;
            cam.allowHDR = true;
            cam.allowMSAA = true;
            cam.nearClipPlane = 0.01f;
            cam.farClipPlane = 100f;

            // Create directional light as child of container
            var lightGO = new GameObject("ThumbnailLight");
            lightGO.hideFlags = HideFlags.HideAndDontSave;
            lightGO.transform.parent = container.transform;
            var light = lightGO.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 0.9f;
            light.transform.rotation = Quaternion.Euler(50, -30, 0);

            // Create RenderTexture
            var rt = new RenderTexture(resolution, resolution, 24, RenderTextureFormat.ARGB32);
            rt.antiAliasing = 8;
            rt.Create();
            cam.targetTexture = rt;

            // Calculate camera position by applying rotation to offset vector
            cam.transform.position =  thing.ThumbnailOffset;
            cam.transform.LookAt(Vector3.zero);

            // Render to texture
            cam.Render();

            // Read pixels from RenderTexture to Texture2D
            RenderTexture.active = rt;
            var tex = new Texture2D(resolution, resolution, TextureFormat.ARGB32, false);
            tex.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
            tex.Apply();

            // Save PNG to Assets folder
            string directory = Path.Combine(Application.dataPath, "Textures");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string path = Path.Combine(directory, thing.name + "_Thumbnail.png");
            File.WriteAllBytes(path, tex.EncodeToPNG());
            Debug.Log($"Thumbnail saved to {path}");

            // Cleanup
            RenderTexture.active = null;
            rt.Release();
            Object.DestroyImmediate(rt);
            Object.DestroyImmediate(container);

            string assetPath = Path.Combine("Assets/Textures/", thing.name + "_Thumbnail.png");
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);

            // Load sprite from asset
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (sprite)
            {
                thing.Thumbnail = sprite;
                EditorUtility.SetDirty(thing);
                AssetDatabase.SaveAssets();
                Debug.Log($"Thumbnail assigned to {thing.name}");
            }
            else
            {
                Debug.LogError("Failed to load generated sprite at: " + assetPath);
            }

            // Refresh project view
            AssetDatabase.Refresh();

        }
    }
}
