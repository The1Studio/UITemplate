namespace TheOneStudio.UITemplate.UITemplate.Utils
{
    using Cysharp.Threading.Tasks;
    using TMPro;
    using UnityEngine;

    public static class LocalizationHelper
    {
        #pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public static async UniTask<string> GetLocalizationString(string tableName, string entryKey)
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
        #pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

        #if THEONE_LOCALIZATION
        public static void SetReferenceString(this TMP_Text textObject, string referenceString, string tableName = "Game")
        {
            if (!textObject.TryGetComponent(out UnityEngine.Localization.Components.LocalizeStringEvent stringEvent)) return;

            if (string.IsNullOrEmpty(stringEvent.StringReference.TableReference))
            {
                stringEvent.StringReference.TableReference = tableName;
            }

            stringEvent.StringReference.TableEntryReference = referenceString;
        }

        public static void SetTextLocalize(this TMP_Text obj, string text, string tableName = "Game")
        {
            obj.SetReferenceString(text);

            obj.text = UnityEngine.Localization.Settings.LocalizationSettings.StringDatabase.GetTable(tableName).GetEntry(text).GetLocalizedString();
        }
        #endif
    }
}