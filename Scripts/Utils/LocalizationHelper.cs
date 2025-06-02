namespace TheOneStudio.UITemplate.UITemplate.Utils
{
    using Cysharp.Threading.Tasks;

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
            }
            #endif
            return entryKey;
        }
        #pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}