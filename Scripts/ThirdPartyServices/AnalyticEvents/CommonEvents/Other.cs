namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents
{
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
        
        public BuildingUnlock(bool success) { this.success = success; }
    }
}