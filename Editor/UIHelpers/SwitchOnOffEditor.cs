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
        private bool isAnimating = false;
        private int currentState = 0;
        private float animationTime = 0f;
        private SerializedProperty timeProp;

        private void OnEnable()
        {
            timeProp = serializedObject.FindProperty("time");
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

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Animating: " + (isAnimating ? "Yes" : "No"));
        }


        private void TurnPositionOnOff(SwitchOnOff SOF, bool state)
        {
            Debug.Log($"SwitchOnOff {SOF}"); 
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
            Debug.Log($"SwitchOnOff {SOF}");
            var onMaterialField = typeof(SwitchOnOff).GetField("on", BindingFlags.NonPublic | BindingFlags.Instance);
            var offMaterialField = typeof(SwitchOnOff).GetField("off", BindingFlags.NonPublic | BindingFlags.Instance);
            var switchRendererField = typeof(SwitchOnOff).GetField("switchRenderer", BindingFlags.NonPublic | BindingFlags.Instance);

            Material onMaterial = (Material)onMaterialField.GetValue(SOF);
            Material offMaterial = (Material)offMaterialField.GetValue(SOF);

            MeshRenderer switchRenderer = (MeshRenderer)switchRendererField.GetValue(SOF);
            switchRenderer.sharedMaterial = state ? onMaterial : offMaterial;
        }



        private void TurnErrorOnOff(SwitchOnOff SOF, int state)
        {
            //SOF.RefreshState();
        }


        private void StartAnimation(SwitchOnOff SOF, int state)
        {
            currentState = state;
            animationTime = 0f;
            isAnimating = true;
        }

        private void UpdateAnimation()
        {
            if (!isAnimating)
                return;

            SwitchOnOff IAC = (SwitchOnOff)target;
            if (IAC == null)
            {
                isAnimating = false;
                return;
            }

            animationTime += Time.deltaTime;
            float timeValue = timeProp.floatValue;
            float t = Mathf.Clamp01(animationTime / timeValue);

            AnimKeyFrameCollection src;
            AnimKeyFrameCollection dst;

            if (currentState == 0) {
                var type = typeof(ImportAnimationComponent);
                var field1 = type.GetField("state0", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                src = (AnimKeyFrameCollection)field1.GetValue(IAC);
                var field2 = type.GetField("state1", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                dst = (AnimKeyFrameCollection)field2.GetValue(IAC);
            }
            else
            {
                var type = typeof(ImportAnimationComponent);
                var field1 = type.GetField("state1", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                src = (AnimKeyFrameCollection)field1.GetValue(IAC);
                var field2 = type.GetField("state0", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                dst = (AnimKeyFrameCollection)field2.GetValue(IAC);
            }

            src.Lerp(dst, t);

            if (t >= 1f)
            {
                isAnimating = false;
            }

            SceneView.RepaintAll(); // Refresh the scene view for visual update
        }

    }
}
