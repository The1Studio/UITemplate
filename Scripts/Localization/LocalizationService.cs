#if THEONE_LOCALIZATION
namespace TheOneStudio.UITemplate.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;
    using UnityEngine.Localization;
    using UnityEngine.Localization.Settings;

    /// <summary>
    /// Handles the actual localization of fields
    /// </summary>
    internal static class LocalizationService
    {
        /// <summary>
        /// Localize all fields in a collection
        /// </summary>
        public static void LocalizeFields(List<LocalizedFieldInfo> localizedFields)
        {
            foreach (var fieldInfo in localizedFields)
            {
                LocalizeField(fieldInfo);
            }
        }

        /// <summary>
        /// Localize a single field
        /// </summary>
        public static void LocalizeField(LocalizedFieldInfo fieldInfo)
        {
            try
            {
                // Get current localization key
                var localizationKey = fieldInfo.MemberInfo.GetMemberValue(fieldInfo.TargetObject) as string;
                
                if (string.IsNullOrEmpty(localizationKey)) return;

                // Get localized value
                var localizedValue = GetLocalizedString(localizationKey);
                
                // Set the localized value back to the field/property
                fieldInfo.MemberInfo.SetMemberValue(fieldInfo.TargetObject, localizedValue);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to localize field {fieldInfo.MemberInfo.Name} in {fieldInfo.TargetObject.GetType().Name}: {ex.Message}");
            }
        }

        /// <summary>
        /// Get localized string from Unity Localization system
        /// </summary>
        public static string GetLocalizedString(string key)
        {
            try
            {
                if (LocalizationSettings.Instance?.GetStringDatabase() == null)
                {
                    Debug.LogWarning($"Localization database not available for key: {key}");
                    return key; // Return the key as fallback
                }

                var localizedString = LocalizationSettings.StringDatabase.GetLocalizedString(key);
                return string.IsNullOrEmpty(localizedString) ? key : localizedString;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to get localized string for key '{key}': {ex.Message}");
                return key; // Return the key as fallback
            }
        }
    }
}
#endif
