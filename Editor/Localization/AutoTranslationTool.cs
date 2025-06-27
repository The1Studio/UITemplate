#if THEONE_LOCALIZATION
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine;

namespace TheOne.Tool.Localization
{
    public class AutoTranslationTool : OdinEditorWindow
    {
        [MenuItem("Tools/TheOne/Auto Translation Tool")]
        private static void OpenWindow()
        {
            GetWindow<AutoTranslationTool>("Auto Translation Tool").Show();
        }

        [Title("ChatGPT Auto Translation")]
        [InlineEditor(InlineEditorModes.LargePreview)]
        [PropertyOrder(0)]
        [InfoBox("This tool only handles auto translation using ChatGPT. Use TMPLocalization for adding entries.", InfoMessageType.Info)]
        public AutoLocalizationConfig config;

        [Title("Translation")]
        [Button("Translate All Missing Entries", ButtonSizes.Large)]
        [GUIColor(0.8f, 0.4f, 0.2f)]
        [PropertyOrder(1)]
        [EnableIf("@config != null")]
        public void TranslateAllMissing()
        {
            if (this.config == null)
            {
                EditorUtility.DisplayDialog("No Config", "Please assign an Auto Localization Config first.", "OK");
                return;
            }

            AutoTranslationManager.TranslateAllMissingEntries(this.config);
            this.RefreshEntries();
        }

        [Title("Current Translation Status")]
        [PropertyOrder(2)]
        [TableList(ShowIndexLabels = true, IsReadOnly = true)]
        public List<TranslationStatusDisplay> entries = new List<TranslationStatusDisplay>();

        [Button("Refresh Status", ButtonSizes.Medium)]
        [PropertyOrder(3)]
        public void RefreshEntries()
        {
            this.entries.Clear();
            var allEntries = AutoTranslationManager.GetAllLocalizationEntries();

            foreach (var entry in allEntries)
            {
                var display = new TranslationStatusDisplay
                {
                    Key = entry.key,
                    Collection = entry.collection,
                    English = entry.translations.ContainsKey("en") ? entry.translations["en"] : "",
                    MissingTranslations = this.GetMissingLanguages(entry)
                };

                this.entries.Add(display);
            }
        }

        private string GetMissingLanguages(LocalizationEntryInfo entry)
        {
            if (this.config == null) return "";

            var missing = new List<string>();
            foreach (var lang in this.config.targetLanguages.Where(l => l.isEnabled))
            {
                if (!entry.translations.ContainsKey(lang.localeCode) ||
                    string.IsNullOrEmpty(entry.translations[lang.localeCode]))
                {
                    missing.Add(lang.language.ToString());
                }
            }

            return missing.Count > 0 ? string.Join(", ", missing) : "✅ Complete";
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            // Auto-load config if exists
            if (this.config == null)
            {
                var configGuids = AssetDatabase.FindAssets("t:AutoLocalizationConfig");
                if (configGuids.Length > 0)
                {
                    var configPath = AssetDatabase.GUIDToAssetPath(configGuids[0]);
                    this.config = AssetDatabase.LoadAssetAtPath<AutoLocalizationConfig>(configPath);
                }
            }

            this.RefreshEntries();
        }
    }

    [System.Serializable]
    public class TranslationStatusDisplay
    {
        [TableColumnWidth(150)]
        public string Key;

        [TableColumnWidth(80)]
        public string Collection;

        [TableColumnWidth(300)]
        [TextArea]
        public string English;

        [TableColumnWidth(200)]
        public string MissingTranslations;
    }
}
#endif
