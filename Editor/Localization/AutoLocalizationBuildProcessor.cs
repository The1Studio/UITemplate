#if THEONE_LOCALIZATION
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization.Tables;

namespace TheOne.Tool.Localization
{
    public class AutoLocalizationBuildProcessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            var config = FindAutoLocalizationConfig();
            if (config == null || !config.autoTranslateOnBuild)
            {
                return;
            }

            Debug.Log("🔍 Auto Localization: Checking missing translations before build...");

            var missingTranslations = CheckMissingTranslations(config);
            
            if (missingTranslations.Count > 0)
            {
                var message = $"Found {missingTranslations.Count} missing translations:\n\n" +
                             string.Join("\n", missingTranslations.Take(10)) +
                             (missingTranslations.Count > 10 ? "\n... and more" : "");

                if (EditorUtility.DisplayDialog("Missing Translations", 
                    $"{message}\n\nContinue building anyway?", 
                    "Continue", "Cancel Build"))
                {
                    Debug.LogWarning($"⚠️ Building with {missingTranslations.Count} missing translations");
                    return;
                }

                throw new BuildFailedException("Build cancelled due to missing translations.");
            }

            Debug.Log("✅ Auto Localization: All translations are complete!");
        }

        private static List<string> CheckMissingTranslations(AutoLocalizationConfig config)
        {
            var missingTranslations = new List<string>();
            var collections = LocalizationEditorSettings.GetStringTableCollections();

            foreach (var collection in collections)
            {
                foreach (var entry in collection.SharedData.Entries)
                {
                    foreach (var targetLang in config.targetLanguages.Where(l => l.isEnabled))
                    {
                        var locale = LocalizationEditorSettings.GetLocales()
                            .FirstOrDefault(l => l.Identifier.Code == targetLang.localeCode);

                        if (locale != null)
                        {
                            var table = collection.GetTable(locale.Identifier) as StringTable;
                            var tableEntry = table?.GetEntry(entry.Id);
                            if (table == null || tableEntry == null || string.IsNullOrEmpty(tableEntry.Value))
                            {
                                missingTranslations.Add($"{entry.Key} ({targetLang.localeCode})");
                            }
                        }
                    }
                }
            }

            return missingTranslations;
        }

        private static AutoLocalizationConfig FindAutoLocalizationConfig()
        {
            var guids = AssetDatabase.FindAssets("t:AutoLocalizationConfig");
            if (guids.Length == 0)
            {
                return null;
            }

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<AutoLocalizationConfig>(path);
        }
    }

    [InitializeOnLoad]
    public static class AutoLocalizationEditorInitializer
    {
        static AutoLocalizationEditorInitializer()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                var config = FindAutoLocalizationConfig();
                if (config != null && config.autoTranslateOnBuild)
                {
                    Debug.Log("Auto Localization: Checking for missing translations in Play Mode...");
                    CheckForMissingTranslations(config);
                }
            }
        }

        private static void CheckForMissingTranslations(AutoLocalizationConfig config)
        {
            var missingTranslations = new List<string>();
            var collections = LocalizationEditorSettings.GetStringTableCollections();

            foreach (var collection in collections)
            {
                foreach (var entry in collection.SharedData.Entries)
                {
                    foreach (var targetLang in config.targetLanguages.Where(l => l.isEnabled))
                    {
                        var locale = LocalizationEditorSettings.GetLocales()
                            .FirstOrDefault(l => l.Identifier.Code == targetLang.localeCode);

                        if (locale != null)
                        {
                            var table = collection.GetTable(locale.Identifier) as StringTable;
                            var tableEntry = table?.GetEntry(entry.Id);
                            if (table == null || tableEntry == null || string.IsNullOrEmpty(tableEntry.Value))
                            {
                                missingTranslations.Add($"{entry.Key} ({targetLang.localeCode})");
                            }
                        }
                    }
                }
            }

            if (missingTranslations.Count > 0)
            {
                Debug.LogWarning($"Auto Localization: Found {missingTranslations.Count} missing translations. " +
                                "Consider running auto-translation before testing.");
            }
        }

        private static AutoLocalizationConfig FindAutoLocalizationConfig()
        {
            var guids = AssetDatabase.FindAssets("t:AutoLocalizationConfig");
            if (guids.Length == 0)
            {
                return null;
            }

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<AutoLocalizationConfig>(path);
        }
    }
}
#endif