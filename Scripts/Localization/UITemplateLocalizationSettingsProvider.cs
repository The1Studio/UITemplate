namespace TheOneStudio.UITemplate.UITemplate.Localization
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using TheOne.Logging;
    using UnityEngine.Localization;
    using UnityEngine.Localization.Settings;
    using UnityEngine.Scripting;
    using ILogger = TheOne.Logging.ILogger;

    public class UITemplateLocalizationSettingsProvider
    {
        private readonly ILogger logger;
        private          string  currentLanguage;
        private          string  tableName;

        public string CurrentLanguage => this.currentLanguage;
        public string TableName       => this.tableName;

        [Preserve] public UITemplateLocalizationSettingsProvider(ILoggerManager loggerManager)
        {
            this.logger          = loggerManager.GetLogger(this);
            this.currentLanguage = "en";
            this.tableName       = "Game";
        }

        public void SetLanguage(string languageCode)
        {
            var locale = FindLocaleByLanguageCode(languageCode);
            LocalizationSettings.SelectedLocale = locale;
            this.currentLanguage                = languageCode;
            this.logger?.Info($"Successfully set language to: {languageCode}");
        }

        public string GetLocalizedText(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                this.logger?.Error($"Localization key is null or empty");
                return key;
            }

            try
            {
                var table = LocalizationSettings.StringDatabase.GetTable(this.tableName);
                if (table == null)
                {
                    this.logger.Error($"String table '{this.tableName}' not found");
                    return key;
                }

                var entry = table.GetEntry(key);
                if (entry == null)
                {
                    this.logger?.Error($"Localization key '{key}' not found in table '{this.tableName}'");
                    return key;
                }

                var localizedString = entry.GetLocalizedString();
                return !string.IsNullOrEmpty(localizedString) ? localizedString : key;
            }
            catch (Exception ex)
            {
                this.logger?.Error($"Error getting localized text for key '{key}': {ex.Message}");
                return key;
            }
        }

        public bool IsLanguageAvailable(string languageCode)
        {
            return FindLocaleByLanguageCode(languageCode) != null;
        }

        public IReadOnlyList<string> AvailableLanguages
        {
            get
            {
                var languages = new List<string>();
                if (LocalizationSettings.AvailableLocales?.Locales != null)
                {
                    foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
                    {
                        var languageCode = locale.Identifier.CultureInfo?.TwoLetterISOLanguageName ?? locale.Identifier.Code;
                        if (!string.IsNullOrEmpty(languageCode))
                        {
                            languages.Add(languageCode);
                        }
                    }
                }
                return languages.AsReadOnly();
            }
        }

        public void SetTableName(string tableName)
        {
            if (!string.IsNullOrEmpty(tableName))
            {
                this.tableName = tableName;
                this.logger?.Info($"Changed table name to: {tableName}");
            }
        }

        public string GetLocalizedText(string tableName, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                this.logger?.Warning("Localization key is null or empty");
                return key;
            }

            if (string.IsNullOrEmpty(tableName))
            {
                this.logger?.Warning("Table name is null or empty, using default table");
                return this.GetLocalizedText(key);
            }

            try
            {
                // Get localized string directly from Unity Localization System using specified table
                var table = LocalizationSettings.StringDatabase.GetTable(tableName);
                if (table == null)
                {
                    this.logger?.Warning($"String table '{tableName}' not found");
                    return key;
                }

                var entry = table.GetEntry(key);
                if (entry == null)
                {
                    this.logger?.Warning($"Localization key '{key}' not found in table '{tableName}'");
                    return key;
                }

                var localizedString = entry.GetLocalizedString();
                return !string.IsNullOrEmpty(localizedString) ? localizedString : key;
            }
            catch (Exception ex)
            {
                this.logger?.Error($"Error getting localized text for key '{key}' from table '{tableName}': {ex.Message}");
                return key;
            }
        }


        private static Locale FindLocaleByLanguageCode(string languageCode)
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