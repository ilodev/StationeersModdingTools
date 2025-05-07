using Assets.Scripts.Objects;
using Assets.Scripts.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    public static class InteractableHelpers
    {

        /// <summary>
        /// Add interactable
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        public static void AddInteractable(Thing thing, string name, InteractableType action)
        {
            Interactable interactable = new Interactable();
            interactable.StringKey = name;
            interactable.Action = action;
            thing.Interactables.Add(interactable);
            EditorUtility.SetDirty(thing);
        }

        public static void AddInteractable(GameObject go, string name, InteractableType action)
        {
            Thing thing = go.GetComponent<Thing>();
            AddInteractable (thing, name, action);  
        }

    }
}
