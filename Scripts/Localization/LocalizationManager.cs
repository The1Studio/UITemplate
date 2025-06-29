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
    [Preserve] public class LocalizationManager : IInitializable
    {
        private readonly SignalBus                       signalBus;
        private readonly BlueprintLocalizationService    blueprintLocalizationService;
        private readonly StringTableLocalizationProvider localizationProvider;

        [Preserve] public LocalizationManager(
            SignalBus                       signalBus,
            BlueprintLocalizationService    blueprintLocalizationService,
            StringTableLocalizationProvider localizationProvider
        )
        {
            this.signalBus                    = signalBus;
            this.blueprintLocalizationService = blueprintLocalizationService;
            this.localizationProvider         = localizationProvider;
        }

        public string CurrentLanguage => this.localizationProvider.CurrentLanguage;

        public System.Collections.Generic.IReadOnlyList<string> AvailableLanguages => this.localizationProvider.AvailableLanguages;

        public void Initialize()
        {
            Debug.Log($"[LocalizationManager] Initialized with language: {this.CurrentLanguage}");
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
                Debug.LogWarning($"[LocalizationManager] Invalid language code: {languageCode}");
                return;
            }

            if (this.CurrentLanguage == languageCode)
            {
                Debug.Log($"[LocalizationManager] Already using language: {languageCode}");
                return;
            }

            var oldLanguage = this.CurrentLanguage;

            try
            {
                this.localizationProvider.SetLanguage(languageCode);
                this.blueprintLocalizationService.LocalizeAllBlueprintFields().Forget();

                this.signalBus.Fire(new LanguageChangedSignal
                {
                    NewLanguage = languageCode,
                    OldLanguage = oldLanguage
                });
            }
            catch (Exception ex)
            {
                Debug.LogError($"[LocalizationManager] Failed to change language to {languageCode}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gets a localized string for the given key
        /// </summary>
        /// <param name="key">Localization key</param>
        /// <param name="fallback">Fallback value if key not found</param>
        /// <returns>Localized string or fallback</returns>
        public string GetLocalizedString(string key, string fallback = null)
        {
            var result = this.localizationProvider.GetLocalizedText(key);
            return result == key && !string.IsNullOrEmpty(fallback) ? fallback : result;
        }

        /// <summary>
        /// Convenience method to set language synchronously (calls async method)
        /// </summary>
        /// <param name="languageCode">Language code to set</param>
        public void SetLanguage(string languageCode)
        {
            this.ChangeLanguage(languageCode);
        }
    }
}