using Objects;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.uihelpers
{

    [CustomEditor(typeof(ActivateButton), true)] 
    public class ActivateButtonEditor : Editor
    {
        private bool currentState = false;

        FieldInfo a0PositionField;
        FieldInfo a1PositionField;
        FieldInfo buttonTransformField;

        ActivateButton AB;

        private void OnEnable()
        {
            AB = (ActivateButton)target;

            a0PositionField = typeof(ActivateButton).GetField("activate0Position", BindingFlags.NonPublic | BindingFlags.Instance);
            a1PositionField = typeof(ActivateButton).GetField("activate1Position", BindingFlags.NonPublic | BindingFlags.Instance);
            buttonTransformField = typeof(ActivateButton).GetField("buttonTransform", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private void OnDisable()
        {
            TurnPositionOnOff(AB, false);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("State Testing", EditorStyles.boldLabel);

            // Test Off position
            if (GUILayout.Button("Activate Off"))
            {
                TurnPositionOnOff(AB, false);
            }

            // Test On Position
            if (GUILayout.Button("Activate On"))
            {
                TurnPositionOnOff(AB, true);
            }
        }

        private void TurnPositionOnOff(ActivateButton AB, bool state)
        {
            currentState = state;

            Vector3 a0Position = (Vector3)a0PositionField.GetValue(AB);
            Vector3 a1Position = (Vector3)a1PositionField.GetValue(AB);
            Transform buttonTransform = (Transform)buttonTransformField.GetValue(AB);

            buttonTransform.localPosition = currentState ? a1Position : a0Position;
        }

    }
}
