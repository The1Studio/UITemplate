namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents
{
    using System.Collections.Generic;
    using Core.AnalyticServices.Data;

    public class TutorialCompletion : IEvent
    {
        public bool   success;
        public string tutorialId;

        public TutorialCompletion(bool success, string tutorialId)
        {
            this.success    = success;
            this.tutorialId = tutorialId;
        }
    }

    public class BuildingUnlock : IEvent
    {
        public bool success;

        public BuildingUnlock(bool success)
        {
            this.success = success;
        }
    }

    public sealed class FTUEStart : IEvent
    {
        public string                     Id         { get; }
        public Dictionary<string, object> Properties { get; }

        public FTUEStart(string id, Dictionary<string, object> properties)
        {
            this.Id         = id;
            this.Properties = properties;
        }
    }

    public sealed class FTUECompleted : IEvent
    {
        public string                     Id         { get; }
        public Dictionary<string, object> Properties { get; }

        public FTUECompleted(string id, Dictionary<string, object> properties)
        {
            this.Id         = id;
            this.Properties = properties;
        }
    }
    
    public sealed class ShowPopupUI : IEvent
    {
        public string Id     { get; }
        public bool   IsAuto { get; }

        public ShowPopupUI(string id, bool isAuto)
        {
            this.Id     = id;
            this.IsAuto = isAuto;
        }
    }
    
    public sealed class ClickWidgetGame : IEvent
    {
        public string                     Id         { get; }
        public Dictionary<string, object> Properties { get; }

        public ClickWidgetGame(string id, Dictionary<string, object> properties)
        {
            this.Id         = id;
            this.Properties = properties;
        }
    }
}