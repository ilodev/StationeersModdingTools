using Assets.Scripts.Objects;
using ilodev.stationeers.moddingtools.diagnostics;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using static RootMotion.Demos.Turret;

namespace ilodev.stationeersmodding.tools.diagnostics
{
    public class ThingOpenEndsPropertyHandler : IPropertyContextMenuHandler
    {
        public void Register(PropertyContextMenuRegistry registry)
        {
            // Individual Slot property
            registry.RegisterHandler("OpenEnds.Array.data", (menu, property, target) =>
            {
                SmallGrid thing = (SmallGrid)target;

                int index = GetArrayIndex(property);
                if (index < 0 || index >= thing.OpenEnds.Count)
                    return;

                Connection connection = thing.OpenEnds[index];

                if (connection.ConnectionType != NetworkType.None)
                    menu.AddItem(new GUIContent("Autopopulate from: Type and Role"), false, () =>
                    {
                        //SetupSlotWithName(thing, slot, slot.Type.ToString());
                        BaseConnectionSetup(thing, connection);
                        EditorUtility.SetDirty(thing);
                    });

                if (connection.ConnectionType != NetworkType.None)
                    menu.AddItem(new GUIContent("Create transform"), false, () =>
                    {
                        BaseConnectionCreate(thing, connection);
                        EditorUtility.SetDirty(thing);
                    });


                if (connection.ConnectionType != NetworkType.None)
                {
                    menu.AddItem(new GUIContent("Reset Connection"), false, () =>
                    {
                        BaseConnectionReset(connection);
                        EditorUtility.SetDirty(thing);
                    });
                }

            });

            /// Global Slots array handler
            registry.RegisterHandler("OpenEnds", (menu, property, target) =>
            {
                SmallGrid thing = (SmallGrid)target;

                menu.AddItem(new GUIContent("Autopopulate connections from hierarchy"), false, () =>
                {
                    EditOrCreateConnectionsFromHiearchy(thing);
                    ///TODO: LOOP THROUGH ALL 
                    EditorUtility.SetDirty(thing);
                });


                /* Too risky to have on a single click                
                menu.AddItem(new GUIContent("Reset"), false, () =>
                {
                    thing.OpenEnds = new System.Collections.Generic.List<Connection>();
                });
                */

                if (thing.OpenEnds.Count == 0)
                    return;

                menu.AddItem(new GUIContent("Autopopulate connections from: Type - Role"), false, () =>
                {
                    foreach (var connection in thing.OpenEnds)
                    {
                        BaseConnectionSetup(thing, connection);
                    }
                    EditorUtility.SetDirty(thing);
                });

            });
        }

        private void EditOrCreateConnectionsFromHiearchy(SmallGrid thing)
        {
            List<Transform> children = FindChildrensWithName(thing.Transform, "Connection");
            foreach(Transform child in children)
            {
                string childName = child.name;
                NetworkType type = ParseNetworkType(childName);
                ConnectionRole role = ParseConnectionRole(childName);
                //Debug.Log($"Child: {childName} | Type: {type} | Role: {role}");
                if (type != NetworkType.None)
                    EditOrCreateConnection(thing, type, role, child);
            }

        }

        // TODO Merge this with BaseConnectionSetup
        void EditOrCreateConnection(SmallGrid thing, NetworkType type, ConnectionRole role, Transform transform)
        {
            var connection = FindConnection(thing.OpenEnds, type, role);
            if (connection == null)
            {
                connection = new Connection(thing);
                connection.ConnectionType = type;
                connection.ConnectionRole = role;
                thing.OpenEnds.Add(connection);
            }
            connection.Parent = thing;
            connection.Transform = transform;
            SphereCollider collider = transform.GetOrAddComponent<SphereCollider>();
            collider.radius = 0.12f;
            collider.isTrigger = true;
            connection.Collider = collider;
        }

        void BaseConnectionCreate(SmallGrid thing, Connection connection)
        {
            string name = "SphereCollider"+ connection.ConnectionType.ToString() + "Connection" + connection.ConnectionRole.ToString() + "Trigger";
            Transform transform = new GameObject(name).transform;
            transform.SetParent(thing.transform);
            transform.localPosition = Vector3.zero;
            EditOrCreateConnection(thing, connection.ConnectionType, connection.ConnectionRole, transform);

        }

        private int GetArrayIndex(SerializedProperty property)
        {
            var match = Regex.Match(property.propertyPath, @"Array\.data\[(\d+)\]");
            return match.Success ? int.Parse(match.Groups[1].Value) : -1;
        }

        private void BaseConnectionReset(Connection connection)
        {
            connection.ConnectionType = NetworkType.None;
            connection.ConnectionRole = ConnectionRole.None;
            connection.Transform = null;
            connection.Collider = null;
            connection.HelperRenderer = null;
        }

        private void BaseConnectionSetup(SmallGrid thing, Connection connection)
        {
            connection.Parent = thing;

            string substring = connection.ConnectionType.ToString() + "Connection" + connection.ConnectionRole.ToString();

            // If location is not set, find by name
            if (connection.Transform == null)
                connection.Transform = FindChildrenWithName(thing.gameObject, substring);

            // If collider is not set, find by name
            if (connection.Collider == null) 
                if (connection.Transform != null)
                {
                    SphereCollider collider = connection.Transform.GetComponentInChildren<SphereCollider>();
                    collider.radius = 0.12f;
                    collider.isTrigger = true;  
                    connection.Collider = collider;
                }

        }

        public static Transform FindChildrenWithName(GameObject parent, string substring)
        {
            if (parent == null) return null;

            foreach (Transform child in parent.transform)
            {
                if (child.name.Contains(substring))
                    return child;

                // Recursive call — searches child's children
                var found = FindChildrenWithName(child.gameObject, substring);
                if (found != null)
                    return found;
            }

            return null; // not found in this branch
        }

        public static List<Transform> FindChildrensWithName(Transform parent, string substring)
        {
            List<Transform> results = new List<Transform>();

            if (parent == null || string.IsNullOrEmpty(substring))
                return results;

            foreach (Transform child in parent)
            {
                if (child.name.IndexOf(substring, System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    results.Add(child);
                }

                // Recursive search in child's children
                results.AddRange(FindChildrensWithName(child, substring));
            }

            return results;
        }

        public static NetworkType ParseNetworkType(string name)
        {
            foreach (NetworkType type in Enum.GetValues(typeof(NetworkType)))
            {
                if (type == NetworkType.None)
                    continue;

                // Check if the enum name is contained in the string
                if (name.IndexOf("Collider" + type.ToString()+ "Connection", StringComparison.OrdinalIgnoreCase) >= 0)
                    return type;
            }
            return NetworkType.None;
        }

        public static ConnectionRole ParseConnectionRole(string name)
        {
            foreach (ConnectionRole role in Enum.GetValues(typeof(ConnectionRole)))
            {
                if (role == ConnectionRole.None)
                    continue;

                if (name.IndexOf("Connection" + role.ToString() + "Trigger", StringComparison.OrdinalIgnoreCase) >= 0)
                    return role;
            }
            return ConnectionRole.None;
        }

        Connection FindConnection(List<Connection> openEnds, NetworkType type, ConnectionRole role)
        {
            return openEnds.Find(conn => conn.ConnectionType == type && conn.ConnectionRole == role);
        }

    }
}
