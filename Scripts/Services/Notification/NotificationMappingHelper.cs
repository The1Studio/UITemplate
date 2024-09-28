namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Cysharp.Threading.Tasks;
    using TheOneStudio.UITemplate.UITemplate.Utils;
    using UnityEngine;
    using UnityEngine.Scripting;

    [Preserve]
    public class NotificationMappingHelper
    {
        private const string Pattern                     = @"{.*?}";
        private const string LocalizationTableCollection = "Notification";

        public async UniTask<string> GetFormatString(string input)
        {
            var regex  = new Regex(Pattern);
            var result = await LocalizationHelper.GetLocalizationString(LocalizationTableCollection, input);

            foreach (Match item in regex.Matches(result))
            {
                result = result.Replace(item.Value, this.GetReplacement(item.Value));
            }

            return result;
        }

        private string GetReplacement(string id)
        {
            id = id.Replace("{", "").Replace("}", "");

            return this.ReplacementCustoms.TryGetValue(id, out var result) ? result : Application.productName;
        }

        public Dictionary<string, string> ReplacementCustoms { get; set; } = new();
    }
}