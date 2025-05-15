using System.Reflection;
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

        BinaryTransformAnimComponent IAC;
        FieldInfo state0;
        FieldInfo state1;

        private void OnEnable()
        {
            IAC = (BinaryTransformAnimComponent)target;
            state0 = typeof(ImportAnimationComponent).GetField("state0", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            state1 = typeof(ImportAnimationComponent).GetField("state1", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            timeProp = serializedObject.FindProperty("time");
 
            EditorApplication.update += UpdateAnimation;
        }

        private void OnDisable()
        {

            // Reset position to state0
            ApplyState(state0);
            EditorApplication.update -= UpdateAnimation;
        }

        private void ApplyState(FieldInfo state)
        {
            AnimKeyFrameCollection src = (AnimKeyFrameCollection)state.GetValue(IAC);
            src.Apply();
            SceneView.RepaintAll(); // Refresh the scene view for visual update
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

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
            animationTime = -0.4f;
            isAnimating = true;
        }

        private void UpdateAnimation()
        {
            if (!isAnimating)
                return;

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
                ApplyState(state0);
                src = (AnimKeyFrameCollection)state0.GetValue(IAC);
                dst = (AnimKeyFrameCollection)state1.GetValue(IAC);
            }
            else
            {
                ApplyState(state1);
                src = (AnimKeyFrameCollection)state1.GetValue(IAC);
                dst = (AnimKeyFrameCollection)state0.GetValue(IAC);
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
