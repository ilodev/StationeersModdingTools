using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UnityEngine;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    public static class AssemblyDefinitionHelpers 
    {

        /// <summary>
        /// Find an assembly definition
        /// </summary>
        /// <param name="startDirectory"></param>
        /// <returns>a path to an asmdef or null</returns>
        public static string FindFirstAsmdef(string startDirectory = "Assets/Scripts")
        {
            var dir = new DirectoryInfo(startDirectory);

            while (dir != null)
            {
                var asmdefFile = dir.GetFiles("*.asmdef", SearchOption.TopDirectoryOnly).FirstOrDefault();
                if (asmdefFile != null)
                    return asmdefFile.FullName;

                dir = dir.Parent;
            }

            return null;
        }

        /// <summary>
        /// Returns the namespace of the first asmdef found in a path
        /// </summary>
        /// <param name="startDirectory"></param>
        /// <returns>Empty string or the found namespace</returns>
        public static string FindAsmdefNamespace(string startDirectory = "Assets/Scripts")
        {
            string rootNamespace = string.Empty;

            string asmdefFile = FindFirstAsmdef(startDirectory);

            if (!string.IsNullOrEmpty(asmdefFile))
            {
                rootNamespace = FindJsonElement(asmdefFile, "rootNamespace");
            }

            return rootNamespace;
        }

        /// <summary>
        /// Returns the name of the first asmdef found in a path
        /// </summary>
        /// <param name="startDirectory"></param>
        /// <returns>Empty string or the found name</returns>
        public static string FindAsmdefAssemblyName(string startDirectory = "Assets/Scripts")
        {
            string name = string.Empty;

            string asmdefFile = FindFirstAsmdef(startDirectory);

            if (!string.IsNullOrEmpty(asmdefFile))
            {
                name = FindJsonElement(asmdefFile, "name");
            }

            return name;
        }


        private static string FindJsonElement(string jsonpath, string name)
        {
            string result = null;

            if (File.Exists(jsonpath))
            {
                string asmdefJson = File.ReadAllText(jsonpath);
                Match match = Regex.Match(asmdefJson, "\""+ name +"\"\\s*:\\s*\"([^\"]+)\"");
                if (match.Success)
                    result = match.Groups[1].Value;
            }
            return result;
        }


    }
}
