namespace TheOneStudio.UITemplate.UITemplate.Localization
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Localization.Signals;
    using TheOneStudio.UITemplate.UITemplate.Utils;
    using UnityEngine;
    using UnityEngine.Localization;
    using UnityEngine.Localization.Settings;
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

        [Preserve] public UITemplateLocalizationManager(
            SignalBus                       signalBus,
            UITemplateLocalizeBlueprint    uiTemplateLocalizeBlueprint
        )
        {
            this.signalBus                    = signalBus;
            this.uiTemplateLocalizeBlueprint = uiTemplateLocalizeBlueprint;
        }


        public string TableName { get; set; } = "en";
        public UniTask LoadLocalizationSettings()
        {
            #if THEONE_LOCALIZATION
            return this.uiTemplateLocalizeBlueprint.LoadCacheOriginalValues(this);
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



            LocalizationSettings.SelectedLocale = this.FindLocaleByLanguageCode(languageCode);
            this.uiTemplateLocalizeBlueprint.LocalizeAllBlueprintFields();

            this.signalBus.Fire(new LanguageChangedSignal
            {
                NewLanguage = languageCode,
            });
        }

        public async UniTask<string> GetLocalizedString(string key, string fallback = null)
        {
            var result = await LocalizationHelper.GetLocalizationString(this.TableName, key);
            return result == key && !string.IsNullOrEmpty(fallback) ? fallback : result;
        }

        public Locale FindLocaleByLanguageCode(string languageCode)
        {
            if (LocalizationSettings.AvailableLocales?.Locales == null) return null;

            foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
            {
                if (locale.Identifier.CultureInfo?.TwoLetterISOLanguageName == languageCode || locale.Identifier.Code == languageCode) return locale;
            }

            return null;
        }
    }
}