using Effects;
using Objects;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.uihelpers
{

    [CustomEditor(typeof(MaterialSetChanger), true)] 
    public class MaterialSetChangerEditor : Editor
    {
        FieldInfo materialSetsField;
        MeshRenderer meshRenderer;
        private SerializedProperty IndexProp;
        MaterialSetChanger MSC;

        private void OnEnable()
        {
            MSC = (MaterialSetChanger)target;

            materialSetsField = typeof(MaterialSetChanger).GetField("_materialSets", BindingFlags.NonPublic | BindingFlags.Instance);
            meshRenderer = MSC.Renderer;
            IndexProp = serializedObject.FindProperty("TargetSetIndex");
        }

        private void OnDisable()
        {
            //SetMaterialSet(MSC, 0);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("State Testing", EditorStyles.boldLabel);

            // Test Off position
            if (GUILayout.Button("Apply Target Set"))
            {
                SetMaterialSet(MSC, IndexProp.intValue);
            }
        }

        private void SetMaterialSet(MaterialSetChanger MSC, int index)
        {
            List<MaterialSet> materialSets = (List<MaterialSet>)materialSetsField.GetValue(MSC);
            Debug.Log($"Sets count {materialSets.Count} {index}");
            materialSets[index].ApplyTo(meshRenderer);
        }

    }
}
