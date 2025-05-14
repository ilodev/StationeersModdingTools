using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    public class AssetWizardData : ScriptableObject
    {
        public MonoScript scriptToAttach;
        public List<Mesh> meshes = new List<Mesh>();
        public bool createAsPrefab;
        public string prefabSavePath = "Assets/Prefabs";
        public GameObject constructorAsset;
        public string namePrefix = "NewAsset";
    }
}