#if THEONE_LOCALIZATION
using UnityEditor;
using UnityEngine;

namespace TheOne.Tool.Localization
{
    public static class LocalizationMenuItems
    {
        [MenuItem("TheOne/Localization/Auto Translation Tool", priority = 2)]
        private static void OpenTranslationTool()
        {
            EditorWindow.GetWindow<AutoTranslationTool>("Auto Translation Tool").Show();
        }

        [MenuItem("TheOne/Localization/Create Config", priority = 11)]
        private static void CreateConfig()
        {
            var config = ScriptableObject.CreateInstance<AutoLocalizationConfig>();

            var path = EditorUtility.SaveFilePanelInProject(
                "Create Auto Localization Config",
                "AutoLocalizationConfig",
                "asset",
                "Choose location for the config file");

            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(config, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                EditorGUIUtility.PingObject(config);
                Selection.activeObject = config;

                Debug.Log($"✅ Created AutoLocalizationConfig at: {path}");
            }
        }

        [MenuItem("TheOne/Localization/TMPLocalization", priority = 1)]
        private static void OpenWindow() { EditorWindow.GetWindow<TMPLocalization>().Show(); }
    }
}
#endif