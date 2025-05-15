using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.uihelpers
{

    [CustomEditor(typeof(BinaryTransformAnimComponent), true)] 
    public class BinaryTransformAnimComponentEditor : Editor
    {
        private bool isAnimating = false;
        private int currentState = 0;
        private float animationTime = 0f;
        private SerializedProperty timeProp;

        private void OnEnable()
        {
            timeProp = serializedObject.FindProperty("time");
            EditorApplication.update += UpdateAnimation;
        }

        private void OnDisable()
        {
            EditorApplication.update -= UpdateAnimation;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            BinaryTransformAnimComponent IAC = (BinaryTransformAnimComponent)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("State Testing", EditorStyles.boldLabel);

            if (GUILayout.Button("Move to State 0"))
            {
                StartAnimation(IAC, 0);
            }

            if (GUILayout.Button("Move to State 1"))
            {
                StartAnimation(IAC, 1);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Animating: " + (isAnimating ? "Yes" : "No"));
        }

        private void StartAnimation(BinaryTransformAnimComponent IAC, int state)
        {
            currentState = state;
            animationTime = -0.5f;
            isAnimating = true;
        }

        private void UpdateAnimation()
        {
            if (!isAnimating)
                return;

            BinaryTransformAnimComponent IAC = (BinaryTransformAnimComponent)target;
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
