namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Sonat
{
    using System.Collections.Generic;
    using Core.AnalyticServices.Data;
    using UnityEngine.Scripting;
    [Preserve]
    internal sealed class TutorialBegin : IEvent
    {
        public string                     Id         { get; }
        public Dictionary<string, object> Properties { get; }

        public TutorialBegin(string id, Dictionary<string, object> properties)
        {
            this.Id         = id;
            this.Properties = properties;
        }
    }
}