namespace TheOneStudio.UITemplate.UITemplate.Localization
{
    using System;
    using Cysharp.Threading.Tasks;
    using GameFoundation.DI;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Utils;
    using UnityEngine.Scripting;

    /// <summary>
    /// Main service for managing language changes and localization
    /// Use this service to change language in your game
    /// </summary>
    [Preserve] public class UITemplateLocalizationManager : IInitializable, IDisposable
    {
        private readonly SignalBus                   signalBus;
        private readonly UITemplateLocalizeBlueprint uiTemplateLocalizeBlueprint;

        [Preserve] public UITemplateLocalizationManager(
            SignalBus                   signalBus,
            UITemplateLocalizeBlueprint uiTemplateLocalizeBlueprint
        )
        {
            this.signalBus                   = signalBus;
            this.uiTemplateLocalizeBlueprint = uiTemplateLocalizeBlueprint;
        }

        public string TableName { get; set; } = "Game";

        public UniTask LoadLocalizationSettings()
        {
            #if THEONE_LOCALIZATION
            return this.uiTemplateLocalizeBlueprint.LoadCacheOriginalValues(this);
            #else
            return UniTask.CompletedTask;
            #endif
        }

        #if THEONE_LOCALIZATION
        public void ChangeLanguage(UnityEngine.Localization.Locale obj)
        {
            this.uiTemplateLocalizeBlueprint.LocalizeAllBlueprintFields();
        }

        public UnityEngine.Localization.Locale GetLocalizationLocale(string languageCode)
        {
            if (UnityEngine.Localization.Settings.LocalizationSettings.AvailableLocales?.Locales == null) return null;

            foreach (var locale in UnityEngine.Localization.Settings.LocalizationSettings.AvailableLocales.Locales)
            {
                if (locale.Identifier.CultureInfo?.TwoLetterISOLanguageName == languageCode || locale.Identifier.Code == languageCode) return locale;
            }

            return null;
        }

        #endif
        public async UniTask<string> GetLocalizedString(string key, string fallback = null)
        {
            var result = await LocalizationHelper.GetLocalizationStringAsync(this.TableName, key);
            return result == key && !string.IsNullOrEmpty(fallback) ? fallback : result;
        }

        public void Initialize()
        {
            #if THEONE_LOCALIZATION
            UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocaleChanged += this.ChangeLanguage;
            #endif
        }

        public void Dispose()
        {
            #if THEONE_LOCALIZATION
            UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocaleChanged -= this.ChangeLanguage;
            #endif
        }
    }
}