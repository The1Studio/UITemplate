namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents
{
    using Core.AnalyticServices.Data;

    public class TutorialCompletion : IEvent
    {
        public bool   Success    { get; set; }
        public string TutorialId { get; set; }
        public int    StepId     { get; set; }
        public string StepName   { get; set; }

        public TutorialCompletion(bool success, string tutorialId, int stepId, string stepName)
        {
            this.Success    = success;
            this.TutorialId = tutorialId;
            this.StepId     = stepId;
            this.StepName   = stepName;
        }
    }
}