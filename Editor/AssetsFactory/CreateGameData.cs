using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Configuration;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    public class CreateGameData 
    {
        [MenuItem("Assets/Create/GameData")]
        static void CreateGameDataXml()
        {
            CreateGameDataEndNameAction create = ScriptableObject.CreateInstance<CreateGameDataEndNameAction>();

            string path = Path.Combine(GetFolder(), "GameData.xml");

            Texture2D icon = EditorGUIUtility.IconContent("TextAsset Icon").image as Texture2D;

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, create, path, icon, null);
        }

        internal class CreateGameDataEndNameAction : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                File.WriteAllText(pathName, string.Empty);

                AssetDatabase.Refresh();
            }
        }

        [MenuItem("Assets/Create/Recipe")]
        static void CreateGameDataRecipeXml()
        {
            CreateGameDataRecipeEndNameAction create = ScriptableObject.CreateInstance<CreateGameDataRecipeEndNameAction>();

            string path = Path.Combine(GetFolder(), "GameDataRecipe.png");

            Texture2D tex = Texture2D.linearGrayTexture;

            Texture2D icon = AssetPreview.GetMiniThumbnail(tex);

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(tex.GetInstanceID(), create, path, icon, null);
        }

        internal class CreateGameDataRecipeEndNameAction : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                Texture2D tex = EditorUtility.InstanceIDToObject(instanceId) as Texture2D;

                byte[] bytes = tex.EncodeToPNG();

                File.WriteAllBytes(pathName, bytes);

                AssetDatabase.Refresh();
            }

            public override void Cancelled(int instanceId, string pathName, string resourceFile)
            {
                Texture2D tex = EditorUtility.InstanceIDToObject(instanceId) as Texture2D;
                Texture2D.DestroyImmediate(tex, true);
            }
        }


        [MenuItem("Assets/Create/GameData2")]
        static void CreateGameData2Xml()
        {
            CreateGameData2EndNameAction create = ScriptableObject.CreateInstance<CreateGameData2EndNameAction>();

            string path = Path.Combine(GetFolder(), "GameData.xml");

            Texture2D icon = EditorGUIUtility.IconContent("TextAsset Icon").image as Texture2D;

            string resource = "TestThis";

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, create, path, icon, resource);
        }

        internal class CreateGameData2EndNameAction : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                File.WriteAllText(pathName, string.Empty);

                AssetDatabase.Refresh();
            }
           
        }




        public static string GetFolder()
        {
            Object[] selectedObjects = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
            if ((selectedObjects?.Length ?? 0) > 0)
            {
                string folderPath = AssetDatabase.GetAssetPath(selectedObjects[0]);
                if (AssetDatabase.IsValidFolder(folderPath)) 
                    return folderPath;
                if (File.Exists(folderPath))
                    return Path.GetDirectoryName(folderPath);
            }
            return "Assets";

        }

    }

}
