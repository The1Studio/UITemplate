namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents
{
    using Core.AnalyticServices.Data;

    public class RewardedAdLoaded : IEvent
    {
        public string placememt;
        public long   loadingMilis;
        public RewardedAdLoaded(string placememt, long loadingMilis)
        {
            this.placememt    = placememt;
            this.loadingMilis = loadingMilis;
        }
    }

    public class RewardedAdLoadFailed : IEvent
    {
        public string placememt;
        public long   loadingMilis;
        public string msg;

        public RewardedAdLoadFailed(string placememt, long loadingMilis, string msg)
        {
            this.placememt    = placememt;
            this.loadingMilis = loadingMilis;
            this.msg          = msg;
        }
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
    
    public class RewardedAdShowFail : IEvent
    {
        public string placememt;
        public RewardedAdShowFail(string placememt) { this.placememt = placememt; }
    }
}