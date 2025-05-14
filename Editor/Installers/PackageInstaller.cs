using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace ilodev.stationeers.moddingtools.installers
{
    public static class PackageInstaller
    {
        private static AddRequest request;
        private static List<string> packagesToInstall;
        private static int currentIndex;

        [MenuItem("Tools/Install Required Packages")]
        public static void InstallPackages()
        {
            if (EditorUtility.DisplayDialog("Install Packages",
                "This will install required packages. Continue?", "Yes", "Cancel"))
            {
                packagesToInstall = new List<string>
            {
                "com.unity.mathematics",
                "com.unity.collections",
                "com.unity.textmeshpro",
                "com.unity.ugui",
            };

                currentIndex = 0;
                InstallNextPackage();
            }
        }

        private static void InstallNextPackage()
        {
            if (currentIndex >= packagesToInstall.Count)
            {
                EditorUtility.ClearProgressBar();
                Debug.Log("All packages installed.");
                return;
            }

            string packageName = packagesToInstall[currentIndex];
            float progress = (float)currentIndex / packagesToInstall.Count;
            EditorUtility.DisplayProgressBar("Installing Packages",
                "Installing: " + packageName, progress);

            request = Client.Add(packageName);
            EditorApplication.update += MonitorRequest;
        }

        private static void MonitorRequest()
        {
            if (request.IsCompleted)
            {
                if (request.Status == StatusCode.Success)
                {
                    Debug.Log("Installed: " + request.Result.packageId);
                }
                else if (request.Status >= StatusCode.Failure)
                {
                    Debug.LogError("Failed to install " + packagesToInstall[currentIndex] + ": " + request.Error.message);
                }

                EditorApplication.update -= MonitorRequest;
                currentIndex++;
                InstallNextPackage();
            }
        }
    }
}
