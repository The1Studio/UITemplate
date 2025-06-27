#if THEONE_LOCALIZATION
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace TheOne.Tool.Localization
{
    /// <summary>
    /// Runtime helper for advanced localization features
    /// </summary>
    public static class RuntimeLocalizationHelper
    {
        private static Dictionary<string, LocalizedString> cachedLocalizedStrings = new Dictionary<string, LocalizedString>();

        /// <summary>
        /// Get a localized string with caching for better performance
        /// </summary>
        public static string GetLocalizedString(string tableName, string key, params object[] args)
        {
            var cacheKey = $"{tableName}:{key}";

            if (!cachedLocalizedStrings.ContainsKey(cacheKey))
            {
                cachedLocalizedStrings[cacheKey] = new LocalizedString(tableName, key);
            }

            var localizedString = cachedLocalizedStrings[cacheKey];

            if (args != null && args.Length > 0)
            {
                return localizedString.GetLocalizedString(args);
            }

            return localizedString.GetLocalizedString();
        }

        /// <summary>
        /// Get a localized string asynchronously
        /// </summary>
        public static async System.Threading.Tasks.Task<string> GetLocalizedStringAsync(string tableName, string key, params object[] args)
        {
            var cacheKey = $"{tableName}:{key}";

            if (!cachedLocalizedStrings.ContainsKey(cacheKey))
            {
                cachedLocalizedStrings[cacheKey] = new LocalizedString(tableName, key);
            }

            var localizedString = cachedLocalizedStrings[cacheKey];

            if (args != null && args.Length > 0)
            {
                var operation = localizedString.GetLocalizedStringAsync(args);
                return await operation.Task;
            }
            else
            {
                var operation = localizedString.GetLocalizedStringAsync();
                return await operation.Task;
            }
        }

        /// <summary>
        /// Change locale at runtime
        /// </summary>
        public static void ChangeLocale(string localeCode)
        {
            var locale = LocalizationSettings.AvailableLocales.Locales.FirstOrDefault(l => l.Identifier.Code == localeCode);
            if (locale != null)
            {
                LocalizationSettings.SelectedLocale = locale;
                Debug.Log($"Locale changed to: {locale.Identifier.Code}");
            }
            else
            {
                Debug.LogWarning($"Locale not found: {localeCode}");
            }
        }

        /// <summary>
        /// Set locale at runtime (alias for ChangeLocale)
        /// </summary>
        public static void SetLocale(string localeCode)
        {
            ChangeLocale(localeCode);
        }

        /// <summary>
        /// Get all available locales
        /// </summary>
        public static List<LocaleInfo> GetAvailableLocales()
        {
            return LocalizationSettings.AvailableLocales.Locales
                .Select(locale => new LocaleInfo
                {
                    Code = locale.Identifier.Code,
                    DisplayName = locale.LocaleName,
                    Locale = locale
                })
                .ToList();
        }

        /// <summary>
        /// Check if a key exists in a table
        /// </summary>
        public static bool KeyExists(string tableName, string key)
        {
            try
            {
                var table = LocalizationSettings.StringDatabase.GetTableAsync(tableName).Result;
                return table?.GetEntry(key) != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Clear the localization cache
        /// </summary>
        public static void ClearCache()
        {
            cachedLocalizedStrings.Clear();
        }

        /// <summary>
        /// Format a localized string with rich text support
        /// </summary>
        public static string FormatLocalizedString(string tableName, string key, Dictionary<string, object> parameters)
        {
            var localizedString = GetLocalizedString(tableName, key);

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    localizedString = localizedString.Replace($"{{{param.Key}}}", param.Value?.ToString() ?? "");
                }
            }

            return localizedString;
        }
    }

    [Serializable]
    public class LocaleInfo
    {
        public string Code;
        public string DisplayName;
        [NonSerialized]
        public Locale Locale;

        public override string ToString()
        {
            return $"{this.DisplayName} ({this.Code})";
        }
    }

    /// <summary>
    /// Component for easy runtime locale switching
    /// </summary>
    public class LocaleSwitcher : MonoBehaviour
    {
        [Header("Locale Settings")]
        [SerializeField] private string defaultLocaleCode = "en";
        [SerializeField] private bool saveSelectedLocale = true;
        [SerializeField] private string localePrefsKey = "SelectedLocale";

        [Header("UI References")]
        [SerializeField] private UnityEngine.UI.Dropdown localeDropdown;

        private List<LocaleInfo> availableLocales;

        private void Start()
        {
            this.InitializeLocaleSwitcher();
        }

        private void InitializeLocaleSwitcher()
        {
            this.availableLocales = RuntimeLocalizationHelper.GetAvailableLocales();

            if (this.localeDropdown != null)
            {
                this.SetupDropdown();
            }

            this.LoadSavedLocale();
        }

        private void SetupDropdown()
        {
            this.localeDropdown.options.Clear();

            foreach (var locale in this.availableLocales)
            {
                this.localeDropdown.options.Add(new UnityEngine.UI.Dropdown.OptionData(locale.ToString()));
            }

            this.localeDropdown.onValueChanged.AddListener(this.OnLocaleChanged);

            // Set current selection
            var currentLocale = LocalizationSettings.SelectedLocale?.Identifier.Code;
            var currentIndex = this.availableLocales.FindIndex(l => l.Code == currentLocale);
            if (currentIndex >= 0)
            {
                this.localeDropdown.value = currentIndex;
            }
        }

        private void OnLocaleChanged(int index)
        {
            if (index >= 0 && index < this.availableLocales.Count)
            {
                var selectedLocale = this.availableLocales[index];
                RuntimeLocalizationHelper.ChangeLocale(selectedLocale.Code);

                if (this.saveSelectedLocale)
                {
                    PlayerPrefs.SetString(this.localePrefsKey, selectedLocale.Code);
                    PlayerPrefs.Save();
                }
            }
        }

        private void LoadSavedLocale()
        {
            if (this.saveSelectedLocale && PlayerPrefs.HasKey(this.localePrefsKey))
            {
                var savedLocale = PlayerPrefs.GetString(this.localePrefsKey);
                RuntimeLocalizationHelper.ChangeLocale(savedLocale);
            }
            else
            {
                RuntimeLocalizationHelper.ChangeLocale(this.defaultLocaleCode);
            }
        }

        public void SetLocale(string localeCode)
        {
            RuntimeLocalizationHelper.ChangeLocale(localeCode);

            if (this.saveSelectedLocale)
            {
                PlayerPrefs.SetString(this.localePrefsKey, localeCode);
                PlayerPrefs.Save();
            }

            // Update dropdown if it exists
            if (this.localeDropdown != null)
            {
                var index = this.availableLocales.FindIndex(l => l.Code == localeCode);
                if (index >= 0)
                {
                    this.localeDropdown.value = index;
                }
            }
        }
    }
}
#endif