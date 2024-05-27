namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents
{
    using Core.AnalyticServices.Data;

    public class InterstitialAdDownloaded : IEvent
    {
        public string placememt;
        public long   loadingMilis;
        public InterstitialAdDownloaded(string placememt, long loadingMilis)
        {
            this.placememt    = placememt;
            this.loadingMilis = loadingMilis;
        }
    }

    public class InterstitialAdLoadFailed : IEvent
    {
        public string placememt;
        public string message;
        public long   loadingMilis;
        public InterstitialAdLoadFailed(string placememt, string message, long loadingMilis)
        {
            this.placememt    = placememt;
            this.message      = message;
            this.loadingMilis = loadingMilis;
        }
    }

    public class InterstitialAdClicked : IEvent
    {
        public string placememt;
        public InterstitialAdClicked(string placememt) { this.placememt = placememt; }
    }

    public class InterstitialAdDisplayed : IEvent
    {
        public string placememt;
        public int    level;
        public InterstitialAdDisplayed(int level, string placememt)
        {
            this.placememt = placememt;
            this.level     = level;
        }
    }

    public class InterstitialAdDisplayedFailed : IEvent
    {
        public string placememt;
        public InterstitialAdDisplayedFailed(string placememt) { this.placememt = placememt; }
    }

    public class InterstitialAdClosed : IEvent
    {
        public string placememt;
        public InterstitialAdClosed(string placememt) { this.placememt = placememt; }
    }

    public class InterstitialAdCalled : IEvent
    {
        public string placememt;
        public InterstitialAdCalled(string placememt) { this.placememt = placememt; }
    }

    public class InterstitialAdEligible : IEvent
    {
        public string placememt;
        public InterstitialAdEligible(string placememt) { this.placememt = placememt; }
    }
}