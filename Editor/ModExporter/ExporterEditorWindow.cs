using UnityEditor;
using UnityEngine;

using System.Linq;
using System.IO;
using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace ilodev.stationeers.moddingtools.exporter
{
    public class ExporterEditorWindow : EditorWindow
    {
        private EditorScriptableSingleton<ExportSettings> exportSettings;
        private ExportSettingsEditor exportSettingsEditor;

        private int selectedTab = 0;
        // private int selectedPath = 0;
        private List<string> addedAsmdefs = new List<string>();

        ExportEditor exportEditor;
        AssemblyEditor assemblyEditor;
        ArtifactEditor artifactEditor;
        DevelopmentEditor developmentEditor;

        public static string GetShortString(string str)
        {
            if (str == null)
            {
                return null;
            }

            var maxWidth = (int)EditorGUIUtility.currentViewWidth - 252;
            var cutoffIndex = Mathf.Max(0, str.Length - 7 - maxWidth / 7);
            var shortString = str.Substring(cutoffIndex);
            if (cutoffIndex > 0)
                shortString = "..." + shortString;
            return shortString;
        }

        [MenuItem("Modding/Export Settings")]
        public static void ShowWindow()
        {
            var window = GetWindow<ExporterEditorWindow>();
            window.titleContent = new GUIContent("LaunchPad Exporter");
            window.minSize = new Vector2(450, 320);
            ExtraAssetsWindow.ShowWindow();
            window.Focus();
        }

        [MenuItem("Modding/Export Mod", false, 20)]
        public static void ExportModMenuItem()
        {
            ExportMod();
        }

        [MenuItem("Modding/Export && Run Mod", false, 20)]
        public static void ExportAndRunModMenuItem()
        {
            ExportMod();
            RunGame();
        }

        private void OnEnable()
        {
            exportSettings = new EditorScriptableSingleton<ExportSettings>();
            exportSettingsEditor = UnityEditor.Editor.CreateEditor(exportSettings.instance) as ExportSettingsEditor;
            assemblyEditor = new AssemblyEditor();
            artifactEditor = new ArtifactEditor();
            developmentEditor = new DevelopmentEditor(exportSettings.instance);
            exportEditor = new ExportEditor();
        }

        /// <summary>
        /// Used to force refreshing the cached list of assemblies
        /// </summary>
        private void OnProjectChange()
        {
            assemblyEditor.ClearCandidates();
        }
        
        private void OnDisable()
        {
            DestroyImmediate(exportSettingsEditor);
        }

        private void DrawExportEditor(ExportSettings settings)
        {
            if (exportEditor.Draw(settings))
            {
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.Space(50);

                if (GUILayout.Button("Export", GUILayout.Height(25)))
                {
                    EditorApplication.delayCall += () => Export.ExportMod(settings);
                }

                GUILayout.Space(5);

                if (GUILayout.Button("Export & Run", GUILayout.Height(25)))
                {
                    EditorApplication.delayCall += () =>
                    {
                        Export.ExportMod(settings);
                        Export.RunGame(settings);
                    };
                }

                GUILayout.Space(50);
                GUILayout.EndHorizontal();

                GUILayout.FlexibleSpace();
            }
        }

        private void OnGUI()
        {
            GUI.enabled = !EditorApplication.isCompiling && !Application.isPlaying;

            var settings = exportSettings.instance;

            var tabs = new string[] { "Export", "Assemblies", "Copy Artifacts", "Development" };

            selectedTab = GUILayout.Toolbar(selectedTab, tabs);

            switch (tabs[selectedTab])
            {
                case "Export":
                    DrawExportEditor(settings);
                    break;
                case "Assemblies":
                    settings.Assemblies = assemblyEditor.Draw(settings);
                    break;
                case "Copy Artifacts":
                    settings.Artifacts = artifactEditor.Draw(settings);
                    break;
                case "Development":
                    developmentEditor.Draw(settings);
                    break;
            }
        }

        public static void ExportMod()
        {
            var singleton = new EditorScriptableSingleton<ExportSettings>();
            Export.ExportMod(singleton.instance);
        }

        public static void RunGame()
        {
            var singleton = new EditorScriptableSingleton<ExportSettings>();
            Export.RunGame(singleton.instance);
        }
    }
}