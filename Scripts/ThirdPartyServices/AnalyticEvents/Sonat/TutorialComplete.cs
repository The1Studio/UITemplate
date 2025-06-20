namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Sonat
{
    using System.Collections.Generic;
    using Core.AnalyticServices.Data;
    using UnityEngine.Scripting;

    internal sealed class TutorialComplete : IEvent
    {
        public string                     Id         { get; }
        public Dictionary<string, object> Properties { get; }

        [Preserve]
        public TutorialComplete(string id, Dictionary<string, object> properties)
        {
            this.Id         = id;
            this.Properties = properties;
        }
    }
}