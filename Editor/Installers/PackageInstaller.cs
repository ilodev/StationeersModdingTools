using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace ilodev.stationeers.moddingtools.installers
{
    public static class PackageInstaller
    {
        private static AddRequest request;

        [MenuItem("Tools/Install Required Packages")]
        public static void InstallPackages()
        {
            // List of packages to install
            string[] packages = new string[]
            {
            "com.unity.mathematics",
            "com.unity.collections",
            "com.unity.textmeshpro",
            "com.unity.ugui",
            "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask",
            "https://github.com/ilodev/StationeersModsExport.git"
            };

            InstallNextPackage(packages, 0);
        }

        private static void InstallNextPackage(string[] packages, int index)
        {
            if (index >= packages.Length)
            {
                Debug.Log("All packages installed.");
                return;
            }

            string packageName = packages[index];
            Debug.Log("Installing package: " + packageName);

            request = Client.Add(packageName);
            EditorApplication.update += () => MonitorRequest(packages, index);
        }

        private static void MonitorRequest(string[] packages, int index)
        {
            if (request.IsCompleted)
            {
                if (request.Status == StatusCode.Success)
                {
                    Debug.Log("Installed: " + request.Result.packageId);
                }
                else if (request.Status >= StatusCode.Failure)
                {
                    Debug.LogError("Failed to install " + packages[index] + ": " + request.Error.message);
                }

                EditorApplication.update -= () => MonitorRequest(packages, index);
                InstallNextPackage(packages, index + 1);
            }
        }
    }
}
