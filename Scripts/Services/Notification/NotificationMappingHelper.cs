namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;

    public class NotificationMappingHelper
    {
        private const    string                                 Pattern = @"{.*?}";
        
        private readonly UITemplateNotificationMappingBlueprint notificationMappingBlueprint;

        public NotificationMappingHelper(UITemplateNotificationMappingBlueprint notificationMappingBlueprint)
        {
            this.notificationMappingBlueprint = notificationMappingBlueprint;
        }

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

            if (this.ReplacementCustoms.TryGetValue(id, out var result)) return result;

            return this.notificationMappingBlueprint.GetDataById(id).Replacement;
        }

        protected virtual Dictionary<string, string> ReplacementCustoms { get; set; } = new();
    }
}