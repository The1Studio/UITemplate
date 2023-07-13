namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents
{
    using Core.AnalyticServices.Data;
    using UnityEditorInternal;

    public class RewardedAdLoaded : IEvent
    {
        public string place;
        public RewardedAdLoaded(string place)
        {
        }
    }
    
    public class RewardedAdLoadFailed : IEvent
    {
        public string place;
        public RewardedAdLoadFailed(string place)
        {
        }
    }
    
    public class RewardedAdLoadClicked : IEvent
    {
        public string place;
        public RewardedAdLoadClicked(string place)
        {
        }
    }
    
    public class RewardedAdDisplayed : IEvent
    {
        public string place;
        public int    level;
        public RewardedAdDisplayed(string place, int level)
        {
        }
    }
    
    public class RewardedAdCompleted : IEvent
    {
        public string place;
        public RewardedAdCompleted(string place)
        {
        }
    }
    
    public class RewardedSkipped : IEvent
    {
        public string place;
        public RewardedSkipped(string place)
        {
        }
    }
    
    public class RewardedAdEligible : IEvent
    {
        public string place;
        public RewardedAdEligible(string place)
        {
        }
    }
    
    public class RewardedAdCalled : IEvent
    {
        public string place;
        public RewardedAdCalled(string place)
        {
        }
    }
    
    public class RewardedAdOffer : IEvent
    {
        public string place;
        public RewardedAdOffer(string place)
        {
        }
    }
}