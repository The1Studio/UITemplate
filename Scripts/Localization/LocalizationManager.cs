namespace TheOneStudio.UITemplate.UITemplate.Localization
{
    using System;
    using Cysharp.Threading.Tasks;
    using GameFoundation.DI;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Localization.Signals;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// Main service for managing language changes and localization
    /// Use this service to change language in your game
    /// </summary>
    [Preserve] public class LocalizationManager
    {
        private readonly SignalBus                       signalBus;
        private readonly BlueprintLocalizationService    blueprintLocalizationService;
        private readonly LocalizationSettingsProvider localizationSettingsProvider;

        [Preserve] public LocalizationManager(
            SignalBus                       signalBus,
            BlueprintLocalizationService    blueprintLocalizationService,
            LocalizationSettingsProvider localizationSettingsProvider
        )
        {
            this.signalBus                    = signalBus;
            this.blueprintLocalizationService = blueprintLocalizationService;
            this.localizationSettingsProvider         = localizationSettingsProvider;
        }

        public string CurrentLanguage => this.localizationSettingsProvider.CurrentLanguage;

        public System.Collections.Generic.IReadOnlyList<string> AvailableLanguages => this.localizationSettingsProvider.AvailableLanguages;


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

            this.localizationSettingsProvider.SetLanguage(languageCode);
            this.blueprintLocalizationService.LocalizeAllBlueprintFields();

            this.signalBus.Fire(new LanguageChangedSignal
            {
                NewLanguage = languageCode,
                OldLanguage = oldLanguage,
            });
        }

        public string GetLocalizedString(string key, string fallback = null)
        {
            var result = this.localizationSettingsProvider.GetLocalizedText(key);
            return result == key && !string.IsNullOrEmpty(fallback) ? fallback : result;
        }
    }
}