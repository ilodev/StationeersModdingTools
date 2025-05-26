using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Objects;

namespace ilodev.stationeers.moddingtools.diagnostics
{
    public class MyExampleHandler : IPropertyContextMenuHandler
    {
        public void Register(PropertyContextMenuRegistry registry)
        {
            registry.RegisterHandler("IsEmergency", (menu, property, target) =>
            {
                menu.AddItem(new GUIContent("Custom Action"), false, () =>
                {
                    Debug.Log($"Action for {property.name} in {(Thing)target}");
                });
            });
        }
    }
}