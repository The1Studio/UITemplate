namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents
{
    using Core.AnalyticServices.Data;

    public class InterstitialAdDownloaded : IEvent
    {
        public string placememt;
        public InterstitialAdDownloaded(string placememt) { this.placememt = placememt; }
    }
    
    public class InterstitialAdLoadFailed : IEvent
    {
        public string placememt;
        public InterstitialAdLoadFailed(string placememt) { this.placememt = placememt; }
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
        public InterstitialAdDisplayed( int level, string placememt)
        {
            this.placememt = placememt;
            this.level = level;
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
    
    public class InterstitialAdCalled: IEvent
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