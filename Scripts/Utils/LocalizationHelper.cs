namespace TheOneStudio.UITemplate.UITemplate.Utils
{
    using Cysharp.Threading.Tasks;
    using TMPro;
    using UnityEngine;

    public static class LocalizationHelper
    {
        public static async UniTask<string> GetLocalizationStringAsync(string tableName, string entryKey)
        {
            #if THEONE_LOCALIZATION
            var table = await UnityEngine.Localization.Settings.LocalizationSettings.StringDatabase.GetTableAsync(tableName);
            if (table != null)
            {
                var entry = table.GetEntry(entryKey);
                if (entry != null) return entry.GetLocalizedString();

                Debug.LogWarning($"[LocalizationHelper] Entry '{entryKey}' not found in table '{tableName}'.");
                return entryKey;
            }
            Debug.LogError($"[LocalizationHelper] Table '{tableName}' not found.");
            #endif
            return entryKey;
        }

        public static string GetLocalizationString(string tableName, string entryKey)
        {
            #if THEONE_LOCALIZATION
            var table = UnityEngine.Localization.Settings.LocalizationSettings.StringDatabase.GetTable(tableName);
            if (table != null)
            {
                var entry = table.GetEntry(entryKey);
                if (entry != null) return entry.GetLocalizedString();

                Debug.LogWarning($"[LocalizationHelper] Entry '{entryKey}' not found in table '{tableName}'.");
                return entryKey;
            }
            Debug.LogError($"[LocalizationHelper] Table '{tableName}' not found.");
            #endif
            return entryKey;
        }

        #if THEONE_LOCALIZATION
        public static void SetReferenceString(this TMP_Text textObject, string referenceString, string tableName = "Game")
        {
            if (!textObject.TryGetComponent(out UnityEngine.Localization.Components.LocalizeStringEvent stringEvent)) return;

            if (string.IsNullOrEmpty(stringEvent.StringReference.TableReference))
            {
                stringEvent.StringReference.TableReference = tableName;
            }

            stringEvent.StringReference.TableEntryReference = referenceString;
            stringEvent.SetEntry(referenceString);
        }

        public static async void SetTextLocalize(this TMP_Text obj, string entryKey, string tableName = "Game")
        {
            obj.textInfo.ClearAllMeshInfo();
            obj.SetReferenceString(entryKey, tableName);
            var localizationString = await GetLocalizationStringAsync(tableName, entryKey);
            obj.text = localizationString;

        }

        public static async void SetTextLocalizeFormat(this TMP_Text obj, string entryKey, string tableName = "Game", params object[] formatArgs)
        {
            obj.textInfo.ClearAllMeshInfo();
            obj.SetReferenceString(entryKey, tableName);
            var localizationString = await GetLocalizationStringAsync(tableName, entryKey);

            if (formatArgs is { Length: > 0 })
            {
                obj.text = string.Format(localizationString, formatArgs);
            }
            else
            {
                obj.text = localizationString;
            }

        }
        #endif
    }
}