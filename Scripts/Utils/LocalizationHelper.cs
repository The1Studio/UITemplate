namespace TheOneStudio.UITemplate.UITemplate.Utils
{
    using Cysharp.Threading.Tasks;

    public static class LocalizationHelper
    {
        public static async UniTask<string> GetLocalizationString(string tableName, string entryKey)
        {
#if THEONE_LOCALIZATION
            var table = await UnityEngine.Localization.Settings.LocalizationSettings.StringDatabase.GetTableAsync(tableName);
            if (table != null)
            {
                var entry = table.GetEntry(entryKey);
                if (entry != null) return entry.GetLocalizedString();
            }
#endif
            return entryKey;
        }
    }
}