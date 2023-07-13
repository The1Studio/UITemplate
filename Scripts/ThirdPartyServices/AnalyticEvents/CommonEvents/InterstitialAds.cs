namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents
{
    using Core.AnalyticServices.Data;

    public class InterstitialAdDownloaded : IEvent
    {
        public string place;
        public InterstitialAdDownloaded(string place) { this.place = place; }
    }
    
    public class InterstitialAdLoadFailed : IEvent
    {
        public string place;
        public InterstitialAdLoadFailed(string place) { this.place = place; }
    }
    
    public class InterstitialAdClicked : IEvent
    {
        public string place;
        public InterstitialAdClicked(string place) { this.place = place; }
    }
    
    public class InterstitialAdDisplayed : IEvent
    {
        public string place;
        public int    level;
        public InterstitialAdDisplayed( int level, string place)
        {
            this.place = place;
            this.level = level;
        }
    }
    
    public class InterstitialAdDisplayedFailed : IEvent
    {
        public string place;
        public InterstitialAdDisplayedFailed(string place) { this.place = place; }
    }
    
    public class InterstitialAdClosed : IEvent
    {
        public string place;
        public InterstitialAdClosed(string place) { this.place = place; }
    }
    
    public class InterstitialAdCalled: IEvent
    {
        public string place;
        public InterstitialAdCalled(string place) { this.place = place; }
    }
    
    public class InterstitialAdEligible : IEvent
    {
        public string place;
        public InterstitialAdEligible(string place) { this.place = place; }
    }
}