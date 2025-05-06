using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    public class CreateMultiConstructor
    {
        private const string DEFAULT_NAME = "NewMultiConstructor";

        [MenuItem("Assets/Create/Stationeers/Constructors/Multi Constructor", false, 1)]
        public static void CreateMultiConstructorPrefab()
        {
            GameObject go = new GameObject(DEFAULT_NAME);
            go.AddComponent<MeshRenderer>();
            go.AddComponent<MeshFilter>();
            go.AddComponent<Rigidbody>();
            go.AddComponent<MeshCollider>();
        }
    }
}
