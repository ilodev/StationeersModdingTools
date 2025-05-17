using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Objects;
using System.Text;
using UnityEditor;

namespace ilodev.stationeersmods.tools.diagnostics
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
                    Debug.Log("Reset");
                });

                menu.AddItem(new GUIContent("Recalculate"), false, () =>
                {
                    Debug.Log("Recalculate");
                });
            });


            registry.RegisterHandler("Blueprint", BlueprintGeneratorHandler);

            registry.RegisterHandler("ThumbnailOffset", ThumbnailHandler);
            registry.RegisterHandler("ThumbnailRotation", ThumbnailHandler);

            registry.RegisterHandler("Thumbnail", ThumbnailGeneratorHandler);

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

        private void BlueprintGeneratorHandler(GenericMenu menu, SerializedProperty property, Object target)
        {
            Thing thing = (Thing)target;
            Debug.Log("Blueprint generator missing");

        }

    }
}
