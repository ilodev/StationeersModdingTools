using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ilodev.stationeers.moddingtools.validation
{

    public class AssetHealthDashboard : EditorWindow
    {
        private List<AssetIssue> issues = new();
        private string searchFilter = "";
        private Vector2 scrollPos;

        [MenuItem("Tools/Asset Health Dashboard")]
        public static void ShowWindow()
        {
            GetWindow<AssetHealthDashboard>("Asset Health");
        }

        void OnGUI()
        {
            GUILayout.Space(10);

            // Scan Button
            if (GUILayout.Button("Scan Assets", GUILayout.Height(30)))
            {
                issues = AssetValidator.ValidateAllAssets();
            }

            GUILayout.Space(10);

            // Search Bar
            GUILayout.BeginHorizontal();
            GUILayout.Label("Search:", GUILayout.Width(140));
            searchFilter = GUILayout.TextField(searchFilter);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            // Fix All Visible Button
            if (issues.Count > 0)
            {
                if (GUILayout.Button("Fix All Visible", GUILayout.Height(25)))
                {
                    foreach (var issue in issues.Where(i =>
                        MatchesFilter(i) && i.fixAction != null).ToList())
                    {
                        issue.fixAction.Invoke();
                        issues.Remove(issue);
                    }
                    AssetDatabase.Refresh();
                }

                GUILayout.Space(10);
            }

            // Scrollable Issue List
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            if (issues.Count == 0)
            {
                GUILayout.Label("No issues found!", EditorStyles.helpBox);
            }
            else
            {
                foreach (var issue in issues.Where(MatchesFilter).ToList())
                {
                    DrawIssueItem(issue);
                }
            }

            EditorGUILayout.EndScrollView();
        }

        // Helper: Check if issue matches current search filter
        private bool MatchesFilter(AssetIssue issue)
        {
            return string.IsNullOrEmpty(searchFilter) ||
                   issue.path.IndexOf(searchFilter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   issue.description.IndexOf(searchFilter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   (issue.category != null && issue.category.IndexOf(searchFilter, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        // Helper: Draw single issue line
        private void DrawIssueItem(AssetIssue issue)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(AssetPreview.GetMiniThumbnail(issue.asset), GUILayout.Width(20), GUILayout.Height(20));

            GUILayout.Label(issue.path, GUILayout.Width(400));
            GUILayout.Label($"[{issue.category}] {issue.description}", GUILayout.Width(500));

            // Fix button — disabled if no fixAction
            GUI.enabled = issue.fixAction != null;
            if (GUILayout.Button("Fix", GUILayout.Width(50)) && issue.fixAction != null)
            {
                issue.fixAction.Invoke();
                issues.Remove(issue);
                AssetDatabase.Refresh();
            }
            GUI.enabled = true;

            // Ping button
            if (GUILayout.Button("Ping", GUILayout.Width(50)))
            {
                EditorGUIUtility.PingObject(issue.asset);
            }

            GUILayout.EndHorizontal();
        }
    }
}