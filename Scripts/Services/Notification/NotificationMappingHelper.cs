namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using UnityEngine;

    public class NotificationMappingHelper
    {
        private const string Pattern = @"{.*?}";

        public string GetFormatString(string input)
        {
            var regex  = new Regex(Pattern);
            var result = input;

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