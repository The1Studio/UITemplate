#if THEONE
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.TheOne
{
    using System;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using UnityEngine.Scripting;

    [Preserve]
    public class TheOneAnalyticEventFactory : BaseAnalyticEventFactory
    {
        public override AnalyticsEventCustomizationConfig AppsFlyerAnalyticsEventCustomizationConfig { get; set; } = new()
        {
            IgnoreEvents = new()
            {
            },
            CustomEventKeys = new()
            {
                { nameof(BannerShown), "af_banner_shown" },
            },
        };

        public override AnalyticsEventCustomizationConfig FireBaseAnalyticsEventCustomizationConfig { get; set; } = new()
        {
            IgnoreEvents = new(),
            CustomEventKeys = new()
            {
                { nameof(AdsRevenueEvent), "ad_impression_abi" },
                { "AdsRevenueSourceId", "ad_platform" },
                { "AdNetwork", "ad_source" },
                { "AdUnit", "ad_unit_name" },
                { "AdFormat", "ad_format" },
                { "Placement", "placement" },
                { "Currency", "currency" },
                { "Revenue", "value" },
                { "Message", "errormsg" },
            },
        };
    }
}
#endif