namespace TheOneStudio.UITemplate.UITemplate.Localization
{
    using System;
    using Cysharp.Threading.Tasks;
    using GameFoundation.DI;
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
        public void ChangeLanguage(Locale obj)
        {
            this.uiTemplateLocalizeBlueprint.LocalizeAllBlueprintFields();
        }

        public Locale GetLocalizationLocale(string languageCode)
        {
            if (LocalizationSettings.AvailableLocales?.Locales == null) return null;

            foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
            {
                if (locale.Identifier.CultureInfo?.TwoLetterISOLanguageName == languageCode || locale.Identifier.Code == languageCode) return locale;
            }

            return null;
        }

        #endif
        public async UniTask<string> GetLocalizedString(string key, string fallback = null)
        {
            var result = await LocalizationHelper.GetLocalizationString(this.TableName, key);
            return result == key && !string.IsNullOrEmpty(fallback) ? fallback : result;
        }

        public void Initialize()
        {
            #if THEONE_LOCALIZATION
            LocalizationSettings.SelectedLocaleChanged += this.ChangeLanguage;
            #endif
        }

        public void Dispose()
        {
            #if THEONE_LOCALIZATION
            LocalizationSettings.SelectedLocaleChanged -= this.ChangeLanguage;
            #endif
        }
    }
}