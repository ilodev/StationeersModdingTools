using Objects;
using Reagents;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.uihelpers
{

    [CustomEditor(typeof(BlendedAnimComponent), true)] 
    public class BlendedAnimComponentEditor : Editor
    {
        public float Ratio = 0f;

        FieldInfo statesField;
        FieldInfo animatedTransformField;

        BlendedAnimComponent BAC;

        private void OnEnable()
        {
            BAC = (BlendedAnimComponent)target;

            statesField = typeof(BlendedAnimComponent).GetField("states", BindingFlags.NonPublic | BindingFlags.Instance);
            animatedTransformField = typeof(BlendedAnimComponent).GetField("animatedTransform", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private void OnDisable()
        {
            OnRatioChange(BAC, 0);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("State Testing", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            Ratio = EditorGUILayout.Slider("Ratio", Ratio, 0f, 1f);
            if (EditorGUI.EndChangeCheck())
            {
                OnRatioChange(BAC, Ratio);
            }

        }

        private void OnRatioChange(BlendedAnimComponent BAC, float ratio)
        {
            ratio = Mathf.Clamp01(ratio);
            List<KeyFrameData> states = (List<KeyFrameData>)statesField.GetValue(BAC);

            KeyFrameData newState = states.Count == 2 ? KeyFrameData.Lerp(states[0], states[1], ratio) : KeyFrameData.Lerp(states, ratio);

            Transform animatedTransform = (Transform)animatedTransformField.GetValue(BAC);
            animatedTransform.localPosition = newState.Position;
            animatedTransform.localRotation = Quaternion.Euler(newState.Rotation);
            if (!(newState.Scale != Vector3.one))
                return;
            animatedTransform.localScale = newState.Scale;
        }

    }
}
