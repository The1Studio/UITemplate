namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System.Text.RegularExpressions;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;

    public class NotificationMappingHelper
    {
        private readonly UITemplateNotificationMappingBlueprint notificationMappingBlueprint;

        public NotificationMappingHelper(UITemplateNotificationMappingBlueprint notificationMappingBlueprint)
        {
            this.notificationMappingBlueprint = notificationMappingBlueprint;
        }

        public string GetFormatString(string input)
        {
            const string pattern = "{.*}";
            var          regex   = new Regex(pattern);
            var          result  = input;

            foreach (Match item in regex.Matches(input))
            {
                input = input.Replace(item.Value, this.GetReplacement(item.Value));
            }

            return result;
        }

        private string GetReplacement(string id)
        {
            id = id.Replace("{", "").Replace("}", "");
            var record = this.notificationMappingBlueprint.GetDataById(id);
            return record?.Replacement;
        }
    }
}