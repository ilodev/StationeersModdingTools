using System.Collections;
using System.Collections.Generic;
using Objects;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Assets.Scripts.Objects;
using Effects;

namespace ilodev.stationeersmods.tools.uihelpers
{

    [CustomEditor(typeof(SwitchOnOff), true)] 
    public class SwitchOnOffEditor : Editor
    {
        private bool Powered = false;
        

        private void OnEnable()
        {
            //EditorApplication.update += UpdateAnimation;
        }

        private void OnDisable()
        {
            //EditorApplication.update -= UpdateAnimation;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SwitchOnOff SOF = (SwitchOnOff)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("State Testing", EditorStyles.boldLabel);

            // Begin checking for changes
            EditorGUI.BeginChangeCheck();

            Powered = EditorGUILayout.Toggle("IsPowered", Powered);

            // End checking for changes
            if (EditorGUI.EndChangeCheck())
            {
                Debug.Log("Powered has changed! New value: " + Powered);
                // You can also perform whatever actions you need here
            }


            if (GUILayout.Button("Switch On"))
            {
                TurnPositionOnOff(SOF, true);
                TurnMaterialOnOff(SOF, true);
            }

            if (GUILayout.Button("Switch Off"))
            {
                TurnPositionOnOff(SOF, false);
                TurnMaterialOnOff(SOF, false);
            }

            if (GUILayout.Button("Error"))
            {
                TurnErrorOnOff(SOF, 1);
                //StartAnimation(SOF, 1);
            }
        }


        private void TurnPositionOnOff(SwitchOnOff SOF, bool state)
        {
            var onPositionField = typeof(SwitchOnOff).GetField("onPosition", BindingFlags.NonPublic | BindingFlags.Instance);
            var offPositionField = typeof(SwitchOnOff).GetField("offPosition", BindingFlags.NonPublic | BindingFlags.Instance);
            var switchTransformField = typeof(SwitchOnOff).GetField("switchTransform", BindingFlags.NonPublic | BindingFlags.Instance);

            Vector3 onPosition = (Vector3)onPositionField.GetValue(SOF);
            Vector3 offPosition = (Vector3)offPositionField.GetValue(SOF);

            Transform switchTransform = (Transform)switchTransformField.GetValue(SOF);
            switchTransform.localRotation = Quaternion.Euler(state ? onPosition : offPosition);
        }

        private void TurnMaterialOnOff(SwitchOnOff SOF, bool state)
        {
            var onMaterialField = typeof(SwitchOnOff).GetField("on", BindingFlags.NonPublic | BindingFlags.Instance);
            var offMaterialField = typeof(SwitchOnOff).GetField("off", BindingFlags.NonPublic | BindingFlags.Instance);
            var onPoweredMaterialField = typeof(SwitchOnOff).GetField("onPowered", BindingFlags.NonPublic | BindingFlags.Instance);
            var switchRendererField = typeof(SwitchOnOff).GetField("switchRenderer", BindingFlags.NonPublic | BindingFlags.Instance);

            Material onMaterial = (Material)onMaterialField.GetValue(SOF);
            Material offMaterial = (Material)offMaterialField.GetValue(SOF);
            Material onPoweredMaterial = (Material)onPoweredMaterialField.GetValue(SOF);

            Material OnMaterial = Powered ? onPoweredMaterial : onMaterial; 

            MeshRenderer switchRenderer = (MeshRenderer)switchRendererField.GetValue(SOF);
            switchRenderer.sharedMaterial = state ? OnMaterial : offMaterial;
        }



        private void TurnErrorOnOff(SwitchOnOff SOF, int state)
        {
            //SOF.RefreshState();
        }

    }
}
