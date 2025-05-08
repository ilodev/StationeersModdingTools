using UnityEditor;

namespace ilodev.stationeers.moddingtools.installers
{
    [InitializeOnLoad]
    public static class QuickTipsAutoLauncher
    {
        private const string Key = "StationeersModdingTools_QuickTips_Shown";

        static QuickTipsAutoLauncher()
        {
            if (!EditorPrefs.GetBool(Key, false))
            {
                EditorApplication.update += ShowQuickTipsOnce;
            }
        }

        private static void ShowQuickTipsOnce()
        {
            EditorApplication.update -= ShowQuickTipsOnce;
            QuickTipsWindow.ShowWindow();
            EditorPrefs.SetBool(Key, true);
        }
    }
}
