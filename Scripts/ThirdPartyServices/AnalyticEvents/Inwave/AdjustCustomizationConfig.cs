#if INWAVE
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Inwave
{
    using System.Collections.Generic;
    using Core.AnalyticServices.Data;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents;

    /// <summary>
    /// Add this code before builder.RegisterUITemplate in GameLifetimeScope:
    /// var adjustConfig = new AdjustCustomizationConfig();
    /// adjustConfig.CustomEventKeys.Add("event_name", "key");
    /// builder.RegisterInstance<AnalyticsEventCustomizationConfig>(adjustConfig);
    /// </summary>
    public class AdjustCustomizationConfig : AnalyticsEventCustomizationConfig
    {
        public AdjustCustomizationConfig()
        {
            this.CustomEventKeys = new Dictionary<string, string>
            {
                { nameof(InterstitialAdEligible), "aj_inters_logicgame" },
                { nameof(InterstitialAdDownloaded), "aj_inters_successfullyloaded" },
                { nameof(InterstitialAdDisplayed), "aj_insters_displayed" },
                { nameof(RewardedAdEligible), "aj_rewarded_logicgame" },
                { nameof(RewardedAdLoaded), "aj_rewarded_successfullyloaded" },
                { nameof(RewardedAdDisplayed), "af_rewarded_displayed" },
            };
        }
    }
}
#endif