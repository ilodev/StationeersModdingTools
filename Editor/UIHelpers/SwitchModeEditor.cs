using System.Collections;
using System.Collections.Generic;
using Objects;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Assets.Scripts.Objects;
using Effects;
using System;
using Reagents;

namespace ilodev.stationeersmods.tools.uihelpers
{

    [CustomEditor(typeof(SwitchMode), true)] 
    public class SwitchModeEditor : SwitchOnOffEditor
    {
        FieldInfo offPoweredMaterialField;
        SwitchMode SM;

        protected override void OnEnable()
        {
            base.OnEnable();
            offPoweredMaterialField = typeof(SwitchMode).GetField("offPowered", BindingFlags.NonPublic | BindingFlags.Instance);
            SM = (SwitchMode)target;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (Powered)
            {
                if (currentState == false)
                {
                    MeshRenderer switchRenderer = (MeshRenderer)switchRendererField.GetValue(SM);
                    Material offPoweredMaterial = (Material)offPoweredMaterialField.GetValue(SM);
                    switchRenderer.sharedMaterial = offPoweredMaterial;
                    SceneView.RepaintAll(); // Refresh the scene view for visual update
                }

            }
        }
    }
}
