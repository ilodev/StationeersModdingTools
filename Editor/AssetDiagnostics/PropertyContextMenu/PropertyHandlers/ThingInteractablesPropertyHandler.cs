using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Objects;
using System.Text;
using UnityEditor;

namespace ilodev.stationeers.moddingtools.diagnostics
{
    public class ThingInteractablesPropertyHandler : IPropertyContextMenuHandler
    {
        public void Register(PropertyContextMenuRegistry registry)
        {
            registry.RegisterHandler("Interactables", (menu, property, target) =>
            {
                Thing thing = (Thing)target;

                if (!HasInteractable(thing, "Open"))
                {
                    menu.AddItem(new GUIContent("Add/Open"), false, () =>
                    {
                        Interactable interactable = CreateInteractable(InteractableType.Open);
                        thing.Interactables.Add(interactable);
                        thing.HasOpenState = true;
                    });
                }
                if (!HasInteractable(thing, "OnOff"))
                {
                    menu.AddItem(new GUIContent("Add/OnOff"), false, () =>
                    {
                        Interactable interactable = CreateInteractable(InteractableType.OnOff);
                        thing.Interactables.Add(interactable);
                        thing.HasOnOffState = true;
                    });
                }
                if (!HasInteractable(thing, "Powered"))
                {
                    menu.AddItem(new GUIContent("Add/Powered"), false, () =>
                    {
                        Interactable interactable = CreateInteractable(InteractableType.Powered);
                        thing.Interactables.Add(interactable);
                        thing.HasPowerState = true;
                    });
                }

            });
        }

        private bool HasInteractable(Thing thing, string name)
        {
            foreach(var interactable in thing.Interactables)
            {
                if (interactable.StringKey == name)
                    return true;
            }
            return false;
        }

        private Interactable CreateInteractable(InteractableType itype, bool JoinInProgressSync = false, bool CanKeyInteract = false)
        {
            Interactable interactable = new Interactable();
            interactable.StringKey = itype.ToString();
            interactable.Action = itype;
            interactable.JoinInProgressSync = JoinInProgressSync;
            interactable.CanKeyInteract = CanKeyInteract;
            return interactable;
        }
    }
}
