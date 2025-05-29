using Assets.Scripts.Objects;
using ilodev.stationeers.moddingtools.diagnostics;
using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmodding.tools.diagnostics
{
    public class ThingSlotsPropertyHandler : IPropertyContextMenuHandler
    {
        public void Register(PropertyContextMenuRegistry registry)
        {
            // Individual Slot property
            registry.RegisterHandler("Slots.Array.data", (menu, property, target) =>
            {
                Thing thing = (Thing)target;

                int index = GetArrayIndex(property);
                if (index < 0 || index >= thing.Slots.Count)
                    return;

                Slot slot = thing.Slots[index];

                if (slot.Type != Slot.Class.None)
                menu.AddItem(new GUIContent("Autopopulate from: Type"), false, () =>
                    {
                        SetupSlotWithName(thing, slot, slot.Type.ToString());
                        EditorUtility.SetDirty(thing);
                    });

                if (slot.StringKey != "")
                    menu.AddItem(new GUIContent("Autopopulate from: Name"), false, () =>
                    {
                        SetupSlotWithName(thing, slot, slot.StringKey);
                        EditorUtility.SetDirty(thing);
                    });

                if (slot.Type != Slot.Class.None)
                {
                    menu.AddItem(new GUIContent("Reset Slot"), false, () =>
                    {
                        BaseSlotReset(slot);
                        EditorUtility.SetDirty(thing);
                    });
                }

            });

            /// Global Slots array handler
            registry.RegisterHandler("Slots", (menu, property, target) =>
            {
                Thing thing = (Thing)target;

                menu.AddItem(new GUIContent("Autopopulate slots from hierarchy"), false, () =>
                {
                    ///TODO: LOOP THROUGH ALL 
                    EditorUtility.SetDirty(thing);
                });



                if (thing.Slots.Count == 0)
                    return;

                menu.AddItem(new GUIContent("Autopopulate slots from: Name"), false, () =>
                {
                    foreach(var slot in thing.Slots)
                    {
                        SetupSlotWithName(thing, slot, slot.StringKey);
                    }
                    EditorUtility.SetDirty(thing);
                });

                menu.AddItem(new GUIContent("Autopopulate slots from: Type"), false, () =>
                {
                    foreach (var slot in thing.Slots)
                    {
                        SetupSlotWithName(thing, slot, slot.Type.ToString());
                    }
                    EditorUtility.SetDirty(thing);
                });

            });
        }

        private void SetupSlotWithName(Thing thing, Slot slot, string name)
        {
            try{
                Slot.Class type = (Slot.Class)Enum.Parse(typeof(Slot.Class), name);
                BaseSlotSetup(thing, slot, type);
            }
            catch { }
        }

        private int GetArrayIndex(SerializedProperty property)
        {
            var match = Regex.Match(property.propertyPath, @"Array\.data\[(\d+)\]");
            return match.Success ? int.Parse(match.Groups[1].Value) : -1;
        }

        private void BaseSlotReset(Slot slot)
        {
            slot.Type = Slot.Class.None;
            slot.StringKey = "";
            slot.StringHash = 0;
            slot.SpecificTypePrefabHash = 0;
            slot.ScaleMultiplier = 0;
            slot.EntityControlMode = Assets.Scripts.MovementController.Mode.Animation;
            slot.UseInternalAtmosphere = false;
            slot.RealWorldScale = false;
            slot.OccupantCastsShadows = false;
            slot.HidesOccupant = false;
            slot.IsHiddenInSeat = false;
            slot.IsInteractable = false;
            slot.IsSwappable = false;   
            slot.AllowDragging = false;
            slot.IsLocked = false;
            slot.Action = InteractableType.Open;
            slot.Location = null;
            slot.Collider = null;
            slot.Size = new Vector3 (0, 0, 0);  

        }

        private void BaseSlotSetup(Thing thing, Slot slot, Slot.Class slotType)
        {
            slot.Type = slotType;
            slot.StringKey = slotType.ToString();
            slot.StringHash = Animator.StringToHash(slotType.ToString());
            slot.SpecificTypePrefabHash = -1;
            slot.ScaleMultiplier = 1;
            slot.EntityControlMode = Assets.Scripts.MovementController.Mode.Seated;
            slot.IsInteractable = true;
            slot.IsSwappable = true;
            slot.Parent = thing;

            // If location is not set, find by name
            if (slot.Location == null)
                slot.Location = FindChildrenWithName(thing.gameObject, slot.StringKey);

            // If collider is not set, find by name
            if (slot.Collider == null) 
                if (slot.Location != null)
                    slot.Collider = slot.Location.GetComponentInChildren<BoxCollider>();
            
            // If size is not set, find by collider
            if (slot.Collider != null)
                slot.Size = slot.Collider.size;

            // TODO: What to do with the action?

        }

        public static Transform FindChildrenWithName(GameObject parent, string childName)
        {
            if (parent == null) return null;

            foreach (Transform child in parent.transform)
            {
                if (child.name.Contains("SlotType"+childName))
                    return child;

                // Recursive call — searches child's children
                var found = FindChildrenWithName(child.gameObject, childName);
                if (found != null)
                    return found;
            }

            return null; // not found in this branch
        }

    }
}
