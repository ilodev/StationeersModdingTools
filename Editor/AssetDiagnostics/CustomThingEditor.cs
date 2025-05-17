using Assets.Scripts.Objects;
using Assets.Scripts.Sound;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using static RootMotion.Demos.Turret;

namespace ilodev.stationeersmods.tools.diagnostics
{
    public class CustomThingEditor : IThingEditor
    {
        private GUIStyle helpBoxStyle;

        public EditorType Type => EditorType.Before;

        public void OnDisable(Object target) { }

        public void OnEnable(Object target) { }

        public int OnInspectorGUI(Object target, int defaultHidden)
        {
            //Debug.Log("CustomThingEditor.OnInspectorGUI");
            Thing thing = target as Thing;

            // Ensure all GameAudioSources have AudioSources
            // Fix AudioEvents
            foreach (GameAudioSource gameAudioSource in thing.AudioSources)
            {
                if (gameAudioSource.AudioSource == null)
                {
                    EditorGUILayout.HelpBox("GameAudioSources don't have an AudioSource attached.", MessageType.Error);
                    break;
                }
            }

            foreach (Interactable interactable in thing.Interactables)
            {
                if (interactable.StringKey == "")
                {
                    EditorGUILayout.HelpBox("You have unnamed Interactables.", MessageType.Error);
                    break;
                }
            }

            if (thing.Slots != null)
            {
                foreach (Slot slot in thing.Slots)
                {
                    if (slot.StringKey == "")
                    {
                        EditorGUILayout.HelpBox("You have unnamed Slots.", MessageType.Error);
                        break;
                    }
                }
            }

            return defaultHidden;
        }

        // Adding custom right-click context menu for the list items
        [MenuItem("CONTEXT/Thing/Toggle All Interactables")]
        private static void ToggleAllInteractables(MenuCommand command)
        {
            Thing manager = (Thing)command.context;
            foreach (var interactable in manager.Interactables)
            {
                //interactable.isInteractable = !interactable.isInteractable;
            }

            EditorUtility.SetDirty(manager);
            Debug.Log("All Interactables toggled.");
        }


        public int OnUpdate(Object target) {

            int result = 0;

            Thing thing = target as Thing;
            if (thing == null)
                return result;

            // Fix prefab Name and Hash
            if (thing.PrefabName != target.name)
            {
                thing.PrefabName = target.name;
                thing.PrefabHash = ComputeCRC32(target.name);
                result++;
            }

            if (thing.Slots != null)
            {
                // Fix Slot names
                foreach (Slot slot in thing.Slots)
                {
                    if (slot.StringKey != null)
                        slot.StringHash = ComputeCRC32(slot.StringKey);
                }
            }

            if (thing.Interactables != null)
            {
                // Fix Interactables
                foreach (Interactable interactable in thing.Interactables)
                {
                    if (interactable.StringKey != null)
                        interactable.StringHash = ComputeCRC32(interactable.StringKey);

                    if (interactable.Parent == null)
                        interactable.Parent = thing;

                    foreach (GameAudioEvent gameAudioEvent in interactable.AssociatedAudioEvents)
                    {
                        if (gameAudioEvent.Name != null)
                            gameAudioEvent.NameHash = ComputeCRC32(gameAudioEvent.Name);
                        if (gameAudioEvent.ClipsData.Name != null)
                            gameAudioEvent.ClipsData.NameHash = ComputeCRC32(gameAudioEvent.ClipsData.Name);
                    }
                }
            }


            // Fix AudioEvents
            foreach (GameAudioEvent gameAudioEvent in thing.AudioEvents)
            {
                if (gameAudioEvent.Name != null)
                    gameAudioEvent.NameHash = ComputeCRC32(gameAudioEvent.Name);

                if (gameAudioEvent.Parent == null) 
                    gameAudioEvent.Parent = thing;
            }

            // Fix GameAudioSources 
            foreach(GameAudioSource gameAudioSource in thing.AudioSources)
            {
                if (gameAudioSource.Name == null || gameAudioSource.Name == "")
                {
                    gameAudioSource.Name = "Unnamed";
                    gameAudioSource.SourceVolume = 1.0f;
                    gameAudioSource.SourcePitch = 1.0f;
                    gameAudioSource.OcclusionType = OcclusionType.Los;
                    gameAudioSource.LocalMixerGroupHash = -1929936868;
                    gameAudioSource.ExternalMixerGroupHash = -1591426066;
                    gameAudioSource.VacuumMixerGroupHash = -710576460;
                    gameAudioSource.OccludedMixerGroupHash = -1140213991;

                    Keyframe[] keys = new Keyframe[3];
                    keys[0] = new Keyframe(0.0f, 0.0f);
                    keys[1] = new Keyframe(0.6f, 0.0f);
                    keys[2] = new Keyframe(1.0f, 1.0f);
                    gameAudioSource.SpatialCurve = new AnimationCurve(keys);

                    Keyframe[] rev = new Keyframe[2];
                    rev[0] = new Keyframe(0.0f, 0.0f);
                    rev[1] = new Keyframe(1.0f, 1.0f);
                    gameAudioSource.ReverbCurve = new AnimationCurve(rev);
                }
            }

            if (thing.Transform == null)
            {
                thing.ThingTransform = thing.transform;
                result++;
            }

            /*
            Debug.Log($"Thumbnail Name {thing.Thumbnail?.name}");
            // If we have a Thumbnail called rename me, change the name to the asset name
            if (thing.Thumbnail != null && thing.Thumbnail.name == "NewPrefabWithScript")
            {
                string thumbnail = AssetDatabase.GetAssetPath(thing.Thumbnail);
                AssetDatabase.RenameAsset(thumbnail, thing.name + ".png");
            }
            */

            return result;
        }

        public void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property, Object target)
        {
            if (target == null)
                return;
            Thing thing = target as Thing;

            if (property.name == "Interactables")
            {
                menu.AddItem(new GUIContent("Add/Open"), false, () =>
                {
                    Debug.Log("Recalculate");
                });
                menu.AddItem(new GUIContent("Add/OnOff"), false, () =>
                {
                    Debug.Log("Recalculate");
                });
            }


            if (property.name == "Bounds")
            {
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
            }

            if (property.name == "Blueprint")
            {
                GameObject blueprint = property.objectReferenceValue as GameObject;
                if (blueprint == null)
                    menu.AddItem(new GUIContent("Generate Blueprint"), false, () =>
                    {
                        Debug.Log("Generate Blueprint");
                    });
                else
                    menu.AddItem(new GUIContent("Regenerate Blueprint"), false, () =>
                    {
                        Debug.Log("Regenerate Blueprint");
                    });
            }

            if (property.name == "Thumbnail")
            {
                GameObject thumbnail = property.objectReferenceValue as GameObject;
                if (thumbnail == null)
                    menu.AddItem(new GUIContent("Generate Thumbnail"), false, () =>
                    {
                        Debug.Log("Generate Thumbnail");
                    });
                else
                    menu.AddItem(new GUIContent("Regenerate Thumbnail"), false, () =>
                    {
                        Debug.Log("Regenerate Thumbnail");
                    });
            }

        }

        private static int ComputeCRC32(string input)
        {
            uint crc = 0xFFFFFFFF;
            byte[] bytes = Encoding.UTF8.GetBytes(input);

            foreach (byte b in bytes)
            {
                crc ^= b;
                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 1) == 1)
                        crc = (crc >> 1) ^ 0xEDB88320;
                    else
                        crc >>= 1;
                }
            }
            return (int)(crc ^ 0xFFFFFFFF);
        }

    }
}
