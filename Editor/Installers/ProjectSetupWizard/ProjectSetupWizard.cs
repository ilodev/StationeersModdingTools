using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ilodev.stationeers.moddingtools.installers
{

    [InitializeOnLoad]
    public class ProjectSetupWizard : EditorWindow
    {
        private static bool hasOpened = false;

        private string projectName;
        private string namespaceName = "";

        private Texture2D banner;

        /// <summary>
        /// Open the wizard on project load
        /// </summary>
        static ProjectSetupWizard()
        {
            // Do not autolaunch if there is a Scripts folder.
            if (Directory.Exists(Path.Combine(Application.dataPath, "Scripts/")))
                return;

            EditorApplication.update += OpenOnLoad;
        }

        private static void OpenOnLoad()
        {
            if (!hasOpened)
            {
                hasOpened = true;
                ShowWindow();
            }
        }

        /// <summary>
        /// Menu option to show the Wizard dialog
        /// </summary>
        [MenuItem("Tools/Project Setup Wizard")]
        public static void ShowWindow()
        {
            var window = GetWindow<ProjectSetupWizard>("Project Setup Wizard");
            window.position = new Rect(100, 100, 410, 240);
            CenterOnMainWindow(window);
        }

        /// <summary>
        /// Centers an EditorWindow to the Editor main window.
        /// </summary>
        /// <param name="win"></param>
        public static void CenterOnMainWindow(EditorWindow win)
        {
            var main = GetEditorMainWindowPos();
            var pos = win.position;
            float w = (main.width - pos.width) * 0.5f;
            float h = (main.height - pos.height) * 0.5f;
            pos.x = main.x + w;
            pos.y = main.y + h;
            win.position = pos;
        }


        /// <summary>
        /// Returns the position of the Editor main window.
        /// </summary>
        /// <returns></returns>
        public static Rect GetEditorMainWindowPos()
        {
            var containerWinType = GetAllDerivedTypes(AppDomain.CurrentDomain, typeof(ScriptableObject))
                .Where(t => t.Name == "ContainerWindow")
                .FirstOrDefault();
            if (containerWinType == null)
                throw new MissingMemberException("Can't find internal type ContainerWindow. Maybe something has changed inside Unity");

            var showModeField = containerWinType.GetField("m_ShowMode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var positionProperty = containerWinType.GetProperty("position", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (showModeField == null || positionProperty == null)
                throw new MissingFieldException("Can't find internal fields 'm_ShowMode' or 'position'. Maybe something has changed inside Unity");

            foreach (var win in Resources.FindObjectsOfTypeAll(containerWinType))
            {
                var showmode = (int)showModeField.GetValue(win);
                if (showmode == 4) // main window
                {
                    var pos = (Rect)positionProperty.GetValue(win, null);
                    return pos;
                }
            }

            throw new NotSupportedException("Can't find internal main window. Maybe something has changed inside Unity");
        }

        static IEnumerable<Type> GetAllDerivedTypes(AppDomain appDomain, Type parentType)
        {
            return appDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(parentType));
        }

        private void OnEnable()
        {
            string projectPath = Application.dataPath;
            projectName = Path.GetFileName(Path.GetDirectoryName(projectPath));

            // Find path of this script
            string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            string scriptDirectory = Path.GetDirectoryName(scriptPath);

            // Load banner from same folder as the script
            string bannerPath = Path.Combine("Packages/ilodev.stationeersmods.tools/Textures/stationeers-long-blk.png").Replace("\\", "/");
            if (EditorGUIUtility.isProSkin)
            {
                bannerPath = Path.Combine("Packages/ilodev.stationeersmods.tools/Textures/stationeers-long-wht.png").Replace("\\", "/");
            }
            // Load banner from Assets/Editor/banner.png
            banner = AssetDatabase.LoadAssetAtPath<Texture2D>(bannerPath);
        }

        private void OnGUI()
        {
            GUILayout.Space(10);

            if (banner != null)
            {
                Rect bannerRect = GUILayoutUtility.GetRect(position.width, 100);
                GUI.DrawTexture(bannerRect, banner, ScaleMode.ScaleToFit);
            }
            else
            {
                EditorGUILayout.LabelField("Project Setup", EditorStyles.boldLabel);
            }

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Project Name");
            projectName = EditorGUILayout.TextField(projectName);

            GUILayout.Space(5);
            EditorGUILayout.LabelField("Namespace");
            namespaceName = EditorGUILayout.TextField(namespaceName);

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Complete Setup", GUILayout.Height(20)))
            {
                PerformSetup();
            }
        }

        private void RenameAssetsAndReplaceStrings(string modName, string modNamespace)
        {
            string packageFolder = "Packages/ilodev.stationeersmods.tools/Editor/Installers/ProjectWizardSetup/";
            // Rename scene file
            string oldScenePath = packageFolder + "Assets/Scenes/ModNameScene.unity";
            string newScenePath = $"Assets/Scenes/{modName}Scene.unity";
            if (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(oldScenePath) != null)
            {
                string error = AssetDatabase.RenameAsset(oldScenePath, $"{modName}Scene");
                if (!string.IsNullOrEmpty(error))
                    Debug.LogError($"Failed to rename scene: {error}");
            }

            // List of template files to process
            string[] templateFiles = new[]
            {
            packageFolder + "Assets/Resources/ExportSettings.asset.tmpl",
            packageFolder + "Assets/Scripts/ModName.asmdef.tmpl",
            packageFolder + "Assets/Scripts/ModName.cs.tmpl",
            packageFolder + "Assets/Scripts/patches/PrefabPatch.cs.tmpl",
            packageFolder + "Assets/About/About.xml.tmpl",
        };

            foreach (string templatePath in templateFiles)
            {
                if (File.Exists(templatePath))
                {
                    string content = File.ReadAllText(templatePath);

                    // Replace placeholders
                    content = content.Replace("##MOD_NAME##", modName);
                    content = content.Replace("##MOD_NAMESPACE##", modNamespace);

                    // Determine new file path
                    string newPath = templatePath.Replace("ModName", modName).Replace(".tmpl", "");

                    // Write processed file
                    File.WriteAllText(newPath, content);

                    // Delete original template
                    File.Delete(templatePath);
                }
                else
                {
                    Debug.LogWarning($"Template file not found: {templatePath}");
                }
            }

            AssetDatabase.Refresh();
        }

        private void PerformSetup()
        {
            RenameAssetsAndReplaceStrings(projectName, namespaceName);

            // Optionally delete Editor folder after setup
            string editorPath = Path.Combine(UnityEngine.Application.dataPath, "Editor");
            if (Directory.Exists(editorPath))
            {
                Directory.Delete(editorPath, true);
                File.Delete(editorPath + ".meta");
            }

            AssetDatabase.Refresh();
            Close();

            EditorUtility.DisplayDialog("Setup Complete", "Project setup is complete.", "OK");
        }
    }
}
