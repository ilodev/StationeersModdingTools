using Assets.Scripts.Objects;
using System;
using System.Linq;
using System.Reflection;
using Trading;
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

            EditorApplication.contextualPropertyMenu += OnPropertyContextMenu;
        }

        private void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property)
        {
            if (target == null)
                return;

            if (property.name != "parentThing")
                return;

            // Parent already setup
            GameObject parentThing = property.objectReferenceValue as GameObject;
            if (parentThing != null)
                return;

            Thing[] things = ((BinaryTransformAnimComponent)target).GetComponentsInParent<Thing>();
            if (things.Length == 0) 
                return;

            Thing parent = things[0];
            if (parent == null) 
                return;

            menu.AddItem(new GUIContent("Assign parent Thing"), false, () =>
            {
                Debug.Log("Testing");
                AssignParentThing(parent, target);
            });

        }

        private void AssignParentThing(Thing parent, UnityEngine.Object target)
        {
            BinaryTransformAnimComponent BTAC = (BinaryTransformAnimComponent)target;
            foreach (Transform child in BTAC.transform)
            {
                string name = child.gameObject.name;

                #region Assign SmallGrid Connections
                SmallGrid smallGrid = parent.GetComponent<SmallGrid>();
                if (smallGrid != null)
                {
                    if (name.Contains("Connection")) {
                        Connection connection = new Connection(smallGrid);
                        if (name.Contains("Chute"))
                            connection.ConnectionType = NetworkType.Chute;

                        if (name.Contains("Input"))
                            connection.ConnectionRole = ConnectionRole.Input;

                        if (name.Contains("Output"))
                            connection.ConnectionRole = ConnectionRole.Output;

                        connection.Transform = child.transform;

                        Collider collider = child.GetComponent<Collider>();
                        if (collider != null)
                            connection.Collider = collider;

                        smallGrid.OpenEnds.Add(connection);
                    }
                }
                #endregion

                #region Assing Lever 
                #endregion
            }

            BTAC.SetParentThing(parent);
        }

        private void OnDisable()
        {

            // Reset position to state0
            ApplyState(state0);
            EditorApplication.update -= UpdateAnimation;
            EditorApplication.contextualPropertyMenu -= OnPropertyContextMenu;
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
