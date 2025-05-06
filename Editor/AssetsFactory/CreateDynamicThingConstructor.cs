using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    public class CreateDynamicConstructor
    {
        private const string DEFAULT_NAME = "NewDynamicConstructor";

        [MenuItem("Assets/Create/Stationeers/Constructors/Dynamic Constructor", false, 1)]
        public static void CreateDynamicConstructorPrefab()
        {
            GameObject go = new GameObject(DEFAULT_NAME);
            go.AddComponent<MeshRenderer>();
            go.AddComponent<MeshFilter>();
            go.AddComponent<Rigidbody>();
            go.AddComponent<MeshCollider>();
        }
    }
}
