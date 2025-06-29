#if THEONE_LOCALIZATION
namespace TheOneStudio.UITemplate.Localization
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Signals;
    using UnityEngine;
    using UnityEngine.Localization.Settings;

    /// <summary>
    /// Service to handle language changes and coordinate with blueprint localization
    /// </summary>
    public class LanguageService
    {
        private readonly SignalBus signalBus;
        private readonly ILogger   logger;

        public LanguageService(SignalBus signalBus, ILogger logger)
        {
            this.signalBus = signalBus;
            this.logger    = logger;
        }

        /// <summary>
        /// Changes the current language and notifies all blueprint objects to update their localized fields
        /// </summary>
        /// <param name="languageCode">Language code (e.g., "en", "vi", "ja")</param>
        public async void ChangeLanguage(string languageCode)
        {
            try
            {
                if (LocalizationSettings.Instance == null)
                {
                    Debug.LogError("LocalizationSettings not initialized");
                    return;
                }

                // Change Unity's localization system language
                var availableLocales = LocalizationSettings.AvailableLocales.Locales;
                var targetLocale = availableLocales.Find(locale => locale.Identifier.Code == languageCode);

                if (targetLocale == null)
                {
                    Debug.LogError($"Language '{languageCode}' not found in available locales");
                    return;
                }

                await LocalizationSettings.InitializationOperation;
                LocalizationSettings.SelectedLocale = targetLocale;

                // Fire signal to update all blueprint localized fields
                this.signalBus.Fire(new LanguageChangedSignal(languageCode));

                Debug.Log($"Language changed to: {languageCode}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to change language to '{languageCode}': {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the current language code
        /// </summary>
        public string GetCurrentLanguage()
        {
            return LocalizationSettings.SelectedLocale?.Identifier.Code ?? "en";
        }
    }
}
#endif
