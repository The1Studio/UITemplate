namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Sonat
{
    using System.Collections.Generic;
    using Core.AnalyticServices.Data;

    internal sealed class TutorialComplete : IEvent
    {
        public string                     Id         { get; }
        public Dictionary<string, object> Properties { get; }

        public TutorialComplete(string id, Dictionary<string, object> properties)
        {
            this.Id         = id;
            this.Properties = properties;
        }
    }
}