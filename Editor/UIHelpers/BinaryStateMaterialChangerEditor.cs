using Objects;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Effects;
using System;

namespace ilodev.stationeersmods.tools.uihelpers
{

    [CustomEditor(typeof(BinaryStateMaterialChanger), true)] 
    public class BinaryStateMaterialChangerEditor : Editor
    {
        FieldInfo state0Field;
        FieldInfo state1Field;
        MeshRenderer meshRenderer;
        BinaryStateMaterialChanger BSMC;
        bool currentState = false;

        void OnEnable()
        {
            state0Field = typeof(BinaryStateMaterialChanger).GetField("_state0", BindingFlags.NonPublic | BindingFlags.Instance);
            state1Field = typeof(BinaryStateMaterialChanger).GetField("_state1", BindingFlags.NonPublic | BindingFlags.Instance);
            BSMC = (BinaryStateMaterialChanger)target;
            meshRenderer = BSMC.Renderer;
        }

        void OnDisable()
        {
            //ChangeMaterials(BSMC, false);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("State Testing", EditorStyles.boldLabel);

            if (GUILayout.Button("Move to State 0"))
            {
                ChangeMaterials(BSMC, false);
            }
            if (GUILayout.Button("Move to State 1"))
            {
                ChangeMaterials(BSMC, true);
            }
        }

        private void ChangeMaterials(BinaryStateMaterialChanger BSMC, bool state)
        {
            currentState = state;

            Material[] state0 = (Material[])state0Field.GetValue(BSMC);
            Material[] state1 = (Material[])state1Field.GetValue(BSMC);

            Material[] materials = currentState ? state1 : state0;

            Material[] sharedMaterials = meshRenderer.sharedMaterials;
            for (int index = 0; index < materials.Length; ++index)
            {
                if (!(materials[index] == null))
                    sharedMaterials[index] = materials[index];
            }
            meshRenderer.materials = sharedMaterials;

            //SceneView.RepaintAll(); // Refresh the scene view for visual update
        }
    }
}
