using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    public class CreateConstructor
    {
        private const string DEFAULT_NAME = "NewConstructor";

        [MenuItem("Assets/Create/Stationeers/Constructors/Constructor", false, 1)]
        public static void CreateConstructorPrefab()
        {
            GameObject go = new GameObject(DEFAULT_NAME);
            go.AddComponent<MeshRenderer>();
            go.AddComponent<MeshFilter>();
            go.AddComponent<Rigidbody>();
            go.AddComponent<MeshCollider>();
        }
    }
}
