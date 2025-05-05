using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Objects;

// Detects new renamed objects and creates OpenEnds connections with them if needed.

[InitializeOnLoad]
public static class PrefabHierarchyWatcher
{
    static PrefabStage currentStage;
    static List<Transform> cachedChildren = new List<Transform>();
    static List<GameObject> pendingNewObjects = new List<GameObject>();

    static PrefabHierarchyWatcher()
    {
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
        PrefabStage.prefabStageOpened += OnPrefabStageOpened;
        PrefabStage.prefabStageClosing += OnPrefabStageClosing;
        EditorApplication.update += ProcessPendingObjects;
    }

    static void OnPrefabStageOpened(PrefabStage stage)
    {
        Debug.Log($"PrefabStage Opened {stage}");
        currentStage = stage;
        CacheChildren();
    }

    static void OnPrefabStageClosing(PrefabStage stage)
    {
        currentStage = null;
        cachedChildren.Clear();
    }

    static void CacheChildren()
    {
        cachedChildren.Clear();
        if (currentStage == null) return;

        foreach (Transform child in currentStage.prefabContentsRoot.transform)
        {
            cachedChildren.Add(child);
        }
    }

    static void OnHierarchyChanged()
    {
        Debug.Log($"OnHiearchyChanged");

        if (currentStage != null)
        {
            List<Transform> currentChildren = new List<Transform>();
            foreach (Transform child in currentStage.prefabContentsRoot.transform)
            {
                Debug.Log($"Adding current {child.name}");
                currentChildren.Add(child);
            }

            var newChildren = currentChildren.Except(cachedChildren).ToList();
            if (newChildren.Count > 0)
            {
                foreach (var newChild in newChildren)
                {
                    Debug.Log($"Adding pending {newChild.name}");
                    pendingNewObjects.Add(newChild.gameObject);
                }
                cachedChildren = currentChildren;
            }

        }
    }

    static void ProcessPendingObjects()
    {
        if (pendingNewObjects.Count == 0) return;

        for (int i = pendingNewObjects.Count - 1; i >= 0; i--)
        {
            var go = pendingNewObjects[i];
            if (go == null)
            {
                pendingNewObjects.RemoveAt(i);
                continue;
            }
            Debug.Log($"Process pending {go.name}");

            // Wait until the object has a name other than "GameObject"
            if (go.name != "GameObject" && go.GetComponentInParent<Structure>() != null)
            {
                SmallGrid smallGrid = go.GetComponentInParent<SmallGrid>();
                
                // In case we need to add an open End
                int index = GetNextOpenEnd(smallGrid);
                // Reset NetworkType flags
                NetworkType node = NetworkType.None;

                ConnectionRole role = ConnectionRole.None;

                if (go.name.Contains("SphereCollider"))
                {
                    if (go.GetComponent<SphereCollider>() == null)
                    {
                        SphereCollider sc = go.AddComponent<SphereCollider>();
                        Debug.Log($"SphereCollider added to {go.name} after rename.");
                        sc.radius = 0.12f;
                    }
                }

                if (go.name.Contains("Trigger"))
                {
                    Collider collider = go.AddComponent<Collider>();
                    if (collider != null)
                    {
                        collider.isTrigger = true;
                    }
                }

                foreach (var kvp in nameToNetworkTypeMap)
                {
                    if (go.name.Contains(kvp.Key))
                    {
                        node |= kvp.Value;
                        Debug.Log($"[{go.name}] Added network type: {kvp.Value}");
                    }
                }

                foreach (var kvp in nameToConnectionRoleMap)
                {
                    if (go.name.Contains(kvp.Key))
                    {
                        role = kvp.Value;
                        Debug.Log($"[{go.name}] Assigned ConnectionRole: {kvp.Value}");
                        break;  // Stop at first match
                    }
                }
                // Optional: log final result
                Debug.Log($"[{go.name}] Final NetworkType: {node}");


                if (node != NetworkType.None)
                {
                    Assets.Scripts.Objects.Connection conn = new Assets.Scripts.Objects.Connection(smallGrid);
                    conn.ConnectionType = node;
                    conn.ConnectionRole = role;
                    conn.Transform = go.transform;
                    conn.Collider = go.GetComponent<SphereCollider>();
                    smallGrid.OpenEnds.Add(conn);
                }

                pendingNewObjects.RemoveAt(i);
            }
        }
    }

    public static int GetNextOpenEnd( SmallGrid smallGrid)
    {
        return smallGrid.OpenEnds.Count;
    }

    private static Dictionary<string, NetworkType> nameToNetworkTypeMap = new Dictionary<string, NetworkType>()
    {
        { "Pipe", NetworkType.Pipe },
        { "Power", NetworkType.Power },
        { "Data", NetworkType.Data },
        { "Chute", NetworkType.Chute },
        { "Elevator", NetworkType.Elevator },
        { "PipeLiquid", NetworkType.PipeLiquid },
        { "LandingPad", NetworkType.LandingPad },
        { "LaunchPad", NetworkType.LaunchPad },
        { "RoboticArmRail", NetworkType.RoboticArmRail }
    };
    static Dictionary<string, ConnectionRole> nameToConnectionRoleMap = new Dictionary<string, ConnectionRole>()
    {
        { "Input2", ConnectionRole.Input2 },   // Order matters if "Input" is a substring of "Input2"
        { "Input", ConnectionRole.Input },
        { "Output2", ConnectionRole.Output2 },
        { "Output", ConnectionRole.Output },
        { "Waste", ConnectionRole.Waste }
    };


}
