using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Objects;
using System.Text;
using UnityEditor;

namespace ilodev.stationeers.moddingtools.diagnostics
{
    public class ThingPropertyHandler : IPropertyContextMenuHandler
    {
        public void Register(PropertyContextMenuRegistry registry)
        {
            registry.RegisterHandler("Bounds", (menu, property, target) =>
            {
                Thing thing = (Thing)target;

                menu.AddItem(new GUIContent("Reset"), false, () =>
                {
                    Bounds bounds = property.boundsValue;
                    bounds.center = Vector3.zero;
                    bounds.min = Vector3.zero;
                    bounds.max = Vector3.zero;
                    thing.Bounds = bounds;
                    EditorUtility.SetDirty(thing);
                    Debug.Log("Reset");
                });

                menu.AddItem(new GUIContent("Recalculate"), false, () =>
                {
                    Debug.Log("Recalculate");
                    CachePrefabBounds(thing);
                    EditorUtility.SetDirty(thing);
                });
            });

            registry.RegisterHandler("DamageState", (menu, property, target) =>
            {
                Thing thing = (Thing)target;

                menu.AddItem(new GUIContent("Preset based on surface area"), false, () =>
                {
                    thing.flashpointTemperature = 375.15f;
                    thing.autoignitionTemperature = 573.15f;
                    thing.BurnTime = thing.SurfaceArea / 5f;
                    thing.ThingHealth = 100 * thing.SurfaceArea;
                    EditorUtility.SetDirty(thing);
                    Debug.Log("Set default damage temps and health");
                });

            });

                registry.RegisterHandler("Blueprint", BlueprintGeneratorHandler);

            registry.RegisterHandler("ThumbnailOffset", ThumbnailHandler);
            registry.RegisterHandler("ThumbnailRotation", ThumbnailHandler);

            registry.RegisterHandler("Thumbnail", ThumbnailGeneratorHandler);

            registry.RegisterHandler("Thumbnails", PaintableThumbnailGeneratorHandler);

        }

        public static void CachePrefabBounds(Thing thing)
        {
            thing.ThingTransform = thing.transform;
            Quaternion rotation = thing.ThingTransform.rotation;
            Vector3 position = thing.ThingTransform.position;
            thing.ThingTransform.rotation = Quaternion.identity;
            thing.ThingTransformPosition = Vector3.zero;
            Bounds bounds = thing.Bounds;
            thing.Bounds.center = Vector3.zero;
            thing.Bounds.extents = Vector3.zero;
            foreach (Renderer renderer in thing.GetComponentsInChildren<Renderer>())
            {
                thing.Bounds.Encapsulate(renderer.bounds);
            }
            thing.ThingTransform.SetPositionAndRotation(position, rotation);
            thing.SurfaceArea = (float)(2.0 * ((double)thing.Bounds.size.x * (double)thing.Bounds.size.y + (double)thing.Bounds.size.y * (double)thing.Bounds.size.z + (double)thing.Bounds.size.z * (double)thing.Bounds.size.x)) * thing.SurfaceAreaScale;
            EditorUtility.SetDirty(thing);
        }

        private void ThumbnailHandler(GenericMenu menu, SerializedProperty property, Object target)
        {
            Thing thing = (Thing)target;

            menu.AddItem(new GUIContent("Save current view"), false, () =>
            {
                if (SceneView.lastActiveSceneView != null)
                {
                    var sceneCamTransform = SceneView.lastActiveSceneView.camera.transform;
                    Vector3 dirToCam = sceneCamTransform.position - thing.transform.position;

                    thing.ThumbnailOffset = dirToCam;
                    thing.ThumbnailRotation = Quaternion.LookRotation(dirToCam);

                    EditorUtility.SetDirty(thing);
                }
            });
        }

        private void ThumbnailGeneratorHandler(GenericMenu menu, SerializedProperty property, Object target)
        {
            Thing thing = (Thing)target;
            menu.AddItem(new GUIContent("Generate"), false, () =>
            {
                ThumbnailRenderer.CaptureThumbnail(thing);
            });

            if (SceneView.lastActiveSceneView != null)
            {
                menu.AddItem(new GUIContent("Generate from current view"), false, () =>
                {
                    var sceneCamTransform = SceneView.lastActiveSceneView.camera.transform;
                    Vector3 dirToCam = sceneCamTransform.position - thing.transform.position;

                    thing.ThumbnailOffset = dirToCam;
                    thing.ThumbnailRotation = Quaternion.LookRotation(dirToCam);

                    EditorUtility.SetDirty(thing);

                    ThumbnailRenderer.CaptureThumbnail(thing);
                });
            }
        }

        private void PaintableThumbnailGeneratorHandler(GenericMenu menu, SerializedProperty property, Object target)
        {
            Thing thing = (Thing)target;
            if (thing.PaintableMaterial)
            {
                menu.AddItem(new GUIContent("Generate color icons"), false, () =>
                {

                });
            }
        }

        private void BlueprintGeneratorHandler(GenericMenu menu, SerializedProperty property, Object target)
        {
            Thing thing = (Thing)target;
            Debug.Log("Blueprint generator missing");

        }

    }
}
