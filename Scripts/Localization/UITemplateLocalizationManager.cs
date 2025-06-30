namespace TheOneStudio.UITemplate.UITemplate.Localization
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Localization.Signals;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// Main service for managing language changes and localization
    /// Use this service to change language in your game
    /// </summary>
    [Preserve]
    public class UITemplateLocalizationManager
    {
        private readonly SignalBus                       signalBus;
        private readonly UITemplateLocalizeBlueprint    uiTemplateLocalizeBlueprint;
        private readonly UITemplateLocalizationSettingsProvider uiTemplateLocalizationSettingsProvider;

        [Preserve] public UITemplateLocalizationManager(
            SignalBus                       signalBus,
            UITemplateLocalizeBlueprint    uiTemplateLocalizeBlueprint,
            UITemplateLocalizationSettingsProvider uiTemplateLocalizationSettingsProvider
        )
        {
            this.signalBus                    = signalBus;
            this.uiTemplateLocalizeBlueprint = uiTemplateLocalizeBlueprint;
            this.uiTemplateLocalizationSettingsProvider         = uiTemplateLocalizationSettingsProvider;
        }

        public string CurrentLanguage => this.uiTemplateLocalizationSettingsProvider.CurrentLanguage;

        public System.Collections.Generic.IReadOnlyList<string> AvailableLanguages => this.uiTemplateLocalizationSettingsProvider.AvailableLanguages;


        public UniTask LoadLocalizationSettings()
        {
            #if THEONE_LOCALIZATION
            return this.uiTemplateLocalizeBlueprint.LoadCacheOriginalValues();
            #else
            return UniTask.CompletedTask;
            #endif
        }
        /// <summary>
        /// Changes the current language
        /// This will trigger localization of all blueprint fields marked with [LocalizableField]
        /// </summary>
        /// <param name="languageCode">Language code to change to (e.g., "en", "vi", "zh")</param>
        public void ChangeLanguage(string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode))
            {
                Debug.LogError($"[LocalizationManager] Invalid language code: {languageCode}");
                return;
            }

            if (this.CurrentLanguage == languageCode)
            {
                Debug.Log($"[LocalizationManager] Already using language: {languageCode}");
                return;
            }

            var oldLanguage = this.CurrentLanguage;

            this.uiTemplateLocalizationSettingsProvider.SetLanguage(languageCode);
            this.uiTemplateLocalizeBlueprint.LocalizeAllBlueprintFields();

            this.signalBus.Fire(new LanguageChangedSignal
            {
                NewLanguage = languageCode,
                OldLanguage = oldLanguage,
            });
        }

        public string GetLocalizedString(string key, string fallback = null)
        {
            var result = this.uiTemplateLocalizationSettingsProvider.GetLocalizedText(key);
            return result == key && !string.IsNullOrEmpty(fallback) ? fallback : result;
        }
    }
}