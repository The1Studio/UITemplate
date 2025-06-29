namespace TheOneStudio.UITemplate.UITemplate.Localization
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using TheOne.Logging;
    using UnityEngine;
    using UnityEngine.Localization;
    using UnityEngine.Localization.Settings;
    using UnityEngine.Localization.Tables;
    using UnityEngine.Scripting;
    using ILogger = TheOne.Logging.ILogger;

    /// <summary>
    /// Simple localization provider using Unity String Tables
    /// </summary>
    public class UnityStringTableLocalizationProvider
    {
        private readonly ILogger logger;
        private readonly Dictionary<string, Dictionary<string, string>> cachedLanguageData;
        private readonly List<string> availableLanguages;

        private string currentLanguage;
        private StringTable currentStringTable;

        public string CurrentLanguage => this.currentLanguage;
        public IReadOnlyList<string> AvailableLanguages => this.availableLanguages.AsReadOnly();

        [Preserve]
        public UnityStringTableLocalizationProvider(ILoggerManager loggerManager)
        {
            this.logger = loggerManager.GetLogger(this);
            this.cachedLanguageData = new Dictionary<string, Dictionary<string, string>>();
            this.availableLanguages = new List<string>();
            this.currentLanguage = "en"; // Default language

            this.InitializeAvailableLanguages();
        }

        public async UniTask LoadLanguageAsync(string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode))
            {
                this.logger?.Error("Language code cannot be null or empty");
                return;
            }

            try
            {
                this.logger?.Info($"Loading language from Unity String Table: {languageCode}");

                // Check if already cached
                if (this.cachedLanguageData.ContainsKey(languageCode))
                {
                    this.currentLanguage = languageCode;
                    this.logger?.Info($"Using cached language data for: {languageCode}");
                    return;
                }

                // Find the locale for the language code
                var locale = this.FindLocaleByLanguageCode(languageCode);
                if (locale == null)
                {
                    this.logger?.Warning($"Locale not found for language code: {languageCode}");
                    return;
                }

                // Load string table for the locale
                await this.LoadStringTableAsync(locale, languageCode);

                this.currentLanguage = languageCode;
                this.logger?.Info($"Successfully loaded language: {languageCode}");
            }
            catch (Exception ex)
            {
                this.logger?.Error($"Failed to load language {languageCode}: {ex.Message}");
                throw;
            }
        }

        public string GetLocalizedText(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                this.logger?.Warning("Localization key is null or empty");
                return key;
            }

            // Try to get from cached data first
            if (this.cachedLanguageData.TryGetValue(this.currentLanguage, out var languageDict) &&
                languageDict.TryGetValue(key, out var localizedText))
            {
                return localizedText;
            }

            // Try to get from current string table
            if (this.currentStringTable != null)
            {
                var entry = this.currentStringTable.GetEntry(key);
                if (entry != null && !string.IsNullOrEmpty(entry.Value))
                {
                    this.CacheLocalizedText(key, entry.Value);
                    return entry.Value;
                }
            }

            // Try using Unity Localization System directly
            var localizedString = new LocalizedString("Default", key);
            if (localizedString.IsEmpty == false)
            {
                var result = localizedString.GetLocalizedString();
                if (!string.IsNullOrEmpty(result))
                {
                    this.CacheLocalizedText(key, result);
                    return result;
                }
            }

            this.logger?.Warning($"Localization not found for key: {key} in language: {this.currentLanguage}");
            return key; // Return key as fallback
        }

        public bool IsLanguageAvailable(string languageCode)
        {
            return this.availableLanguages.Contains(languageCode);
        }

        public void ClearCache()
        {
            this.cachedLanguageData.Clear();
            this.currentStringTable = null;
            this.logger?.Info("Localization cache cleared");
        }

        private void InitializeAvailableLanguages()
        {
            try
            {
                if (LocalizationSettings.AvailableLocales?.Locales != null)
                {
                    this.availableLanguages.Clear();
                    foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
                    {
                        var languageCode = locale.Identifier.CultureInfo?.TwoLetterISOLanguageName ?? locale.Identifier.Code;
                        if (!string.IsNullOrEmpty(languageCode) && !this.availableLanguages.Contains(languageCode))
                        {
                            this.availableLanguages.Add(languageCode);
                        }
                    }

                    this.logger?.Info($"Initialized {this.availableLanguages.Count} available languages: {string.Join(", ", this.availableLanguages)}");
                }
                else
                {
                    this.logger?.Warning("Unity Localization Settings not found");
                    this.availableLanguages.AddRange(new[] { "en", "vi", "es" });
                }
            }
            catch (Exception ex)
            {
                this.logger?.Error($"Failed to initialize available languages: {ex.Message}");
                this.availableLanguages.AddRange(new[] { "en", "vi", "es" });
            }
        }

        private Locale FindLocaleByLanguageCode(string languageCode)
        {
            if (LocalizationSettings.AvailableLocales?.Locales == null)
                return null;

            foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
            {
                if (locale.Identifier.CultureInfo?.TwoLetterISOLanguageName == languageCode ||
                    locale.Identifier.Code == languageCode)
                    return locale;
            }

            return null;
        }

        private async UniTask LoadStringTableAsync(Locale locale, string languageCode, string tableName = "Default")
        {
            try
            {
                var tableCollection = LocalizationSettings.StringDatabase?.DefaultTable;
                if (tableCollection == null)
                {
                    this.logger?.Error("Default string table collection not found");
                    return;
                }

                var stringTable = await LocalizationSettings.StringDatabase.GetTableAsync(tableCollection.TableCollectionNameReference, locale);
                if (stringTable == null)
                {
                    this.logger?.Warning($"String table not found for locale: {locale.Identifier.Code}");
                    return;
                }

                this.currentStringTable = stringTable;

                var languageDict = new Dictionary<string, string>();
                foreach (var entry in stringTable.Values)
                {
                    if (entry != null && !string.IsNullOrEmpty(entry.Key) && !string.IsNullOrEmpty(entry.Value))
                    {
                        languageDict[entry.Key] = entry.Value;
                    }
                }

                this.cachedLanguageData[languageCode] = languageDict;
                this.logger?.Info($"Cached {languageDict.Count} entries for language: {languageCode}");
            }
            catch (Exception ex)
            {
                this.logger?.Error($"Failed to load string table for locale {locale.Identifier.Code}: {ex.Message}");
                throw;
            }
        }

        private void CacheLocalizedText(string key, string value)
        {
            if (!this.cachedLanguageData.ContainsKey(this.currentLanguage))
            {
                this.cachedLanguageData[this.currentLanguage] = new Dictionary<string, string>();
            }

            this.cachedLanguageData[this.currentLanguage][key] = value;
        }
    }
}