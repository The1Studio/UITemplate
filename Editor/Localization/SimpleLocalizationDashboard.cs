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
    public class SimpleLocalizationDashboard : OdinEditorWindow
    {
        [MenuItem("Tools/TheOne/Simple Localization Dashboard")]
        private static void OpenWindow()
        {
            GetWindow<SimpleLocalizationDashboard>("Simple Localization Dashboard").Show();
        }

        [Title("Configuration")]
        [InlineEditor(InlineEditorModes.LargePreview)]
        [PropertyOrder(0)]
        public AutoLocalizationConfig config;

        [Title("Add New Entry")]
        [PropertyOrder(1)]
        [LabelText("Key")]
        public string newKey = "";

        [PropertyOrder(2)]
        [LabelText("English Text")]
        [TextArea(2, 5)]
        public string newEnglishText = "";

        [Button("Add Entry to String Tables", ButtonSizes.Large)]
        [GUIColor(0.3f, 0.8f, 0.3f)]
        [PropertyOrder(3)]
        [EnableIf("@!string.IsNullOrEmpty(newKey) && !string.IsNullOrEmpty(newEnglishText)")]
        public void AddNewEntry()
        {
            AutoLocalizationManager.AddLocalizationEntry(this.newKey, this.newEnglishText);
            this.newKey = "";
            this.newEnglishText = "";
            this.RefreshEntries();
        }

        [Title("Translation")]
        [Button("Translate All Missing Entries", ButtonSizes.Large)]
        [GUIColor(0.8f, 0.4f, 0.2f)]
        [PropertyOrder(4)]
        [EnableIf("@config != null")]
        public void TranslateAllMissing()
        {
            if (this.config == null)
            {
                EditorUtility.DisplayDialog("No Config", "Please assign an Auto Localization Config first.", "OK");
                return;
            }

            AutoLocalizationManager.TranslateAllMissingEntries(this.config);
            this.RefreshEntries();
        }

        [Title("Current Entries")]
        [PropertyOrder(5)]
        [TableList(ShowIndexLabels = true, IsReadOnly = true)]
        public List<LocalizationEntryDisplay> entries = new List<LocalizationEntryDisplay>();

        [Button("Refresh Entries", ButtonSizes.Medium)]
        [PropertyOrder(6)]
        public void RefreshEntries()
        {
            this.entries.Clear();
            var allEntries = AutoLocalizationManager.GetAllLocalizationEntries();

            foreach (var entry in allEntries)
            {
                var display = new LocalizationEntryDisplay
                {
                    Key = entry.key,
                    Collection = entry.collection,
                    English = entry.translations.ContainsKey("en") ? entry.translations["en"] : "",
                    Vietnamese = entry.translations.ContainsKey("vi") ? entry.translations["vi"] : "",
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

        [Title("Quick Setup")]
        [Button("🚀 Quick Setup Wizard", ButtonSizes.Large)]
        [PropertyOrder(-1)]
        public void RunQuickSetup()
        {
            var setupWindow = GetWindow<QuickSetupWindow>("Quick Setup");
            setupWindow.parentDashboard = this;
            setupWindow.Show();
        }
    }

    [System.Serializable]
    public class LocalizationEntryDisplay
    {
        [TableColumnWidth(150)]
        public string Key;

        [TableColumnWidth(80)]
        public string Collection;

        [TableColumnWidth(200)]
        [TextArea]
        public string English;

        [TableColumnWidth(200)]
        [TextArea]
        public string Vietnamese;

        [TableColumnWidth(150)]
        public string MissingTranslations;
    }

    public class QuickSetupWindow : OdinEditorWindow
    {
        public SimpleLocalizationDashboard parentDashboard;

        [Title("Quick Entry Creation")]
        [LabelText("How many entries do you want to add?")]
        public int entryCount = 5;

        [LabelText("Key Prefix")]
        public string keyPrefix = "ui_";

        [Button("Generate Sample Entries")]
        public void GenerateSampleEntries()
        {
            var sampleEntries = new string[]
            {
                "Welcome to the game!",
                "Start Game",
                "Settings",
                "Quit",
                "Loading...",
                "Game Over",
                "Try Again",
                "High Score",
                "Pause",
                "Resume"
            };

            for (int i = 0; i < this.entryCount && i < sampleEntries.Length; i++)
            {
                var key = $"{this.keyPrefix}{sampleEntries[i].ToLower().Replace(" ", "_").Replace("!", "").Replace(".", "")}";
                AutoLocalizationManager.AddLocalizationEntry(key, sampleEntries[i]);
            }

            if (this.parentDashboard != null)
            {
                this.parentDashboard.RefreshEntries();
            }

            EditorUtility.DisplayDialog("Sample Entries Added", $"Added {this.entryCount} sample entries!", "OK");
            this.Close();
        }
    }
}
#endif