namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents
{
    using System.Collections.Generic;
    using Core.AnalyticServices.Data;

    /// <summary> Event for resource earning </summary>
    public class EarnResource : IEvent
    {
        /// <summary> Resource id.</summary>
        public string ResourceId { get; set; }
        
        /// <summary> Earned value.</summary>
        public long   Value      { get; set; }
        
        /// <summary> Earn from where.</summary>
        public string Source     { get; set; }
        
        /// <summary> Resource spent to earn.</summary>
        public Dictionary<string, object> SpentResources { get; set; }
        
        /// <summary> Timestamp of the event.</summary>
        public long Timestamp { get; set; }

        public EarnResource(string resourceId, long value, string source, Dictionary<string,object> spentResources,long timestamp)
        {
            this.ResourceId     = resourceId;
            this.Value          = value;
            this.Source         = source;
            this.SpentResources = spentResources;
            this.Timestamp      = timestamp;
        }
    }

    /// <summary> Event for resource earning </summary>
    public class SpendResource : IEvent
    {
        /// <summary> Resource id.</summary>
        public string ResourceId { get; set; }
        
        /// <summary> Spend value.</summary>
        public long   Value      { get; set; }
        
        /// <summary> Spend to where.</summary>
        public string Location   { get; set; }
        
        /// <summary> Timestamp of the event.</summary>
        public long   Timestamp  { get; set; }

        public SpendResource(string resourceId, long value, string location, long timestamp)
        {
            this.ResourceId = resourceId;
            this.Value      = value;
            this.Location   = location;
            this.Timestamp  = timestamp;
        }
    }
}