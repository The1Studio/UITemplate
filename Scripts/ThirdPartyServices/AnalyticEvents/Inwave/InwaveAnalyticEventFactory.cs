#if INWAVE
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Inwave
{
    using System;
    using System.Collections.Generic;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using GameFoundation.Signals;
    using UnityEngine.Scripting;

    public class InwaveAnalyticEventFactory : BaseAnalyticEventFactory
    {
        [Preserve]
        public InwaveAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticServices)
            : base(signalBus, analyticServices)
        {
        }

        public override void ForceUpdateAllProperties() { }

        public override AnalyticsEventCustomizationConfig AppsFlyerAnalyticsEventCustomizationConfig { get; set; } = new()
        {
            IgnoreEvents = new HashSet<Type>()
            {
                typeof(GameStarted)
            },
            CustomEventKeys = new Dictionary<string, string>()
            {
                { nameof(BannerShown), "af_banner_shown" }
            }
        };

        public override AnalyticsEventCustomizationConfig FireBaseAnalyticsEventCustomizationConfig { get; set; } = new()
        {
            IgnoreEvents = new HashSet<Type>(),
            CustomEventKeys = new Dictionary<string, string>()
            {
                { nameof(AdsRevenueEvent), "ad_impression_new" },
                { "AdsRevenueSourceId", "ad_platform" },
                { "AdNetwork", "ad_source" },
                { "AdUnit", "ad_unit_name" },
                { "AdFormat", "ad_format" },
                { "Placement", "placement" },
                { "Currency", "currency" },
                { "Revenue", "value" },
                { "Message", "errormsg" },
            }
        };
    }
}
#endif