namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents
{
    using Core.AnalyticServices.Data;

    public class RewardedAdLoaded : IEvent
    {
        public string placememt;
        public RewardedAdLoaded(string placememt) { this.placememt = placememt; }
    }

    public class RewardedAdLoadFailed : IEvent
    {
        public string placememt;
        public RewardedAdLoadFailed(string placememt) { this.placememt = placememt; }
    }

    public class RewardedAdLoadClicked : IEvent
    {
        public string placememt;
        public RewardedAdLoadClicked(string placememt) { this.placememt = placememt; }
    }

    public class RewardedAdDisplayed : IEvent
    {
        public string placememt;
        public int    level;
        public RewardedAdDisplayed(string placememt, int level)
        {
            this.placememt = placememt;
            this.level     = level;
        }
    }

    public class RewardedAdCompleted : IEvent
    {
        public string placememt;
        public RewardedAdCompleted(string placememt) { this.placememt = placememt; }
    }

    public class RewardedSkipped : IEvent
    {
        public string placememt;
        public RewardedSkipped(string placememt) { this.placememt = placememt; }
    }

    public class RewardedAdEligible : IEvent
    {
        public string placememt;
        public RewardedAdEligible(string placememt) { this.placememt = placememt; }
    }

    public class RewardedAdCalled : IEvent
    {
        public string placememt;
        public RewardedAdCalled(string placememt) { this.placememt = placememt; }
    }

    public class RewardedAdOffer : IEvent
    {
        public string placememt;
        public RewardedAdOffer(string placememt) { this.placememt = placememt; }
    }
}