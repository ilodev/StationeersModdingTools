using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Items;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeers.moddingtools.diagnostics
{
    public class CustomDynamicThingEditor : IThingEditor
    {
        public EditorType Type => EditorType.Before;

        public void OnDisable(Object target) { }

        public void OnEnable(Object target) { }

        public int OnInspectorGUI(Object target, int defaultHidden)
        {
            //Debug.Log("CustomDynamicThingEditor.OnInspectorGUI");
            DynamicThing dynamicThing = target as DynamicThing;

            if (dynamicThing == null)
                return defaultHidden;

            MeshRenderer mr = dynamicThing.GetComponent<MeshRenderer>();
            MeshFilter mf = dynamicThing.GetComponent<MeshFilter>();
            if (mr == null || mf == null)
            {
                EditorGUILayout.HelpBox("This DynamicThing is missing a MeshRenderer or a MeshFilter", MessageType.Info);
            }

            Collider col = dynamicThing.GetComponent<Collider>();
            if (col == null)
            {
                EditorGUILayout.HelpBox("This dynamicThing is missing a Collider", MessageType.Info);
            }

            if (dynamicThing.Blueprint == null)
                EditorGUILayout.HelpBox("This dynamicThing is missing a Blueprint", MessageType.Error);

            if (dynamicThing.Thumbnail == null)
                EditorGUILayout.HelpBox("This dynamicThing is missing a Thumbnail", MessageType.Error);

            Rigidbody rb = dynamicThing.GetComponent<Rigidbody>();
            if (rb == null)
            {
                EditorGUILayout.HelpBox("This DynamicThing is missing a RigidBody", MessageType.Error);
            }

            Constructor constructor = dynamicThing.GetComponent<Constructor>();
            if (constructor != null)
            {
                if (constructor.BuildStructure == null)
                {
                    EditorGUILayout.HelpBox("This Constructor is missing a structure Prefab to build", MessageType.Error);
                }
            }

            MultiConstructor multiConstructor = dynamicThing.GetComponent<MultiConstructor>();
            if (multiConstructor != null)
            {
                if (multiConstructor.Constructables?.Count == 0)
                {
                    EditorGUILayout.HelpBox("This MultiConstructor is missing a structure Prefab to build", MessageType.Error);
                }
            }

            DynamicThingConstructor dynamicThingConstructor = dynamicThing.GetComponent<DynamicThingConstructor>();
            if (dynamicThingConstructor != null)
            {
                if (dynamicThingConstructor.ConstructedPrefab == null)
                {
                    EditorGUILayout.HelpBox("This DynamicThingConstructor is missing a Prefab to build", MessageType.Error);
                }
            }


            return defaultHidden;
        }



        public int OnUpdate(Object target) {

            int result = 0;

            DynamicThing dynamicThing = target as DynamicThing;

            if (dynamicThing == null) 
                return result;

            if (dynamicThing.RigidBody == null)
            {
                dynamicThing.RigidBody = dynamicThing.GetComponent<Rigidbody>();
                result++;
            }

            return result;
        }

    }
}
