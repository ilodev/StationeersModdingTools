using System.Collections;
using System.Collections.Generic;
using Objects;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Assets.Scripts.Objects;
using Effects;
using System;

namespace ilodev.stationeersmods.tools.uihelpers
{

    [CustomEditor(typeof(SwitchOnOff), true)] 
    public class SwitchOnOffEditor : Editor
    {
        private bool Powered = false;
        private bool currentState = false;
        private bool Error = false;
        private float animationTime = 0f;
        private int animationStep = 0;

        FieldInfo onPositionField;
        FieldInfo offPositionField;
        FieldInfo switchTransformField;

        FieldInfo onMaterialField;
        FieldInfo offMaterialField;
        FieldInfo onPoweredMaterialField;
        FieldInfo switchRendererField;

        FieldInfo errorMaterialField;

        SwitchOnOff SOF;

        private void OnEnable()
        {
            SOF = (SwitchOnOff)target;

            onPositionField = typeof(SwitchOnOff).GetField("onPosition", BindingFlags.NonPublic | BindingFlags.Instance);
            offPositionField = typeof(SwitchOnOff).GetField("offPosition", BindingFlags.NonPublic | BindingFlags.Instance);
            switchTransformField = typeof(SwitchOnOff).GetField("switchTransform", BindingFlags.NonPublic | BindingFlags.Instance);

            onMaterialField = typeof(SwitchOnOff).GetField("on", BindingFlags.NonPublic | BindingFlags.Instance);
            offMaterialField = typeof(SwitchOnOff).GetField("off", BindingFlags.NonPublic | BindingFlags.Instance);
            onPoweredMaterialField = typeof(SwitchOnOff).GetField("onPowered", BindingFlags.NonPublic | BindingFlags.Instance);
            switchRendererField = typeof(SwitchOnOff).GetField("switchRenderer", BindingFlags.NonPublic | BindingFlags.Instance);

            errorMaterialField = typeof(SwitchOnOff).GetField("error", BindingFlags.NonPublic | BindingFlags.Instance);

            EditorApplication.update += UpdateErrorAnimation;
        }

        private void OnDisable()
        {
            TurnPositionOnOff(SOF, false);
            EditorApplication.update -= UpdateErrorAnimation;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("State Testing", EditorStyles.boldLabel);

            // Test Powered property
            EditorGUI.BeginChangeCheck();
            Powered = EditorGUILayout.Toggle("IsPowered", Powered);
            if (EditorGUI.EndChangeCheck())
            {
                TurnPositionOnOff(SOF, currentState);
            }

            // Test On Position
            if (GUILayout.Button("Switch On"))
            {
                TurnPositionOnOff(SOF, true);
            }

            // Test Off position
            if (GUILayout.Button("Switch Off"))
            {
                TurnPositionOnOff(SOF, false);
            }

            // Toggle error state
            if (GUILayout.Button("Error"))
            {
                TurnErrorOnOff(SOF, !Error);
            }
        }

        private void TurnPositionOnOff(SwitchOnOff SOF, bool state)
        {
            Error = false;
            currentState = state;

            Vector3 onPosition = (Vector3)onPositionField.GetValue(SOF);
            Vector3 offPosition = (Vector3)offPositionField.GetValue(SOF);
            Transform switchTransform = (Transform)switchTransformField.GetValue(SOF);

            switchTransform.localRotation = Quaternion.Euler(state ? onPosition : offPosition);

            TurnMaterialOnOff(SOF, state);
        }

        private void TurnMaterialOnOff(SwitchOnOff SOF, bool state)
        {
            Material onMaterial = (Material)onMaterialField.GetValue(SOF);
            Material offMaterial = (Material)offMaterialField.GetValue(SOF);
            Material onPoweredMaterial = (Material)onPoweredMaterialField.GetValue(SOF);

            Material OnMaterial = Powered ? onPoweredMaterial : onMaterial; 

            MeshRenderer switchRenderer = (MeshRenderer)switchRendererField.GetValue(SOF);
            switchRenderer.sharedMaterial = state ? OnMaterial : offMaterial;
        }

        private void TurnErrorOnOff(SwitchOnOff SOF, bool state)
        {
            Error = state;
            animationTime = 0f;

            if (Error == false)
                TurnPositionOnOff(SOF, currentState);

            // UpdateErrorAnimation will take care of the current error material
        }

        // Cycle through all error materials every 0.3 seconds
        private void UpdateErrorAnimation()
        {
            if (!Error)
                return;

            if (SOF == null)
            { 
                Error = false;
                return;
            }

            Material[] errorMaterials = (Material[])errorMaterialField.GetValue(SOF);
            if (errorMaterials.Length == 0)
                return;

            animationTime += Time.deltaTime;
            //Debug.Log($"{animationTime}");
            if (animationTime > 50.0f) 
            {
                if (animationStep >= errorMaterials.Length - 1)
                    animationStep = 0;
                else
                    animationStep +=1;

                MeshRenderer switchRenderer = (MeshRenderer)switchRendererField.GetValue(SOF);
                switchRenderer.sharedMaterial = errorMaterials[animationStep];

                animationTime = 0f;
            }
        }
    }
}
