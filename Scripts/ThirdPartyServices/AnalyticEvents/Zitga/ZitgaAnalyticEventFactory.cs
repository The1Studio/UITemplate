#if ZITGA

namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Zitga
{
    using System;
    using System.Collections.Generic;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using GameFoundation.Signals;
    using UnityEngine.Scripting;

    public class ZitgaAnalyticEventFactory : BaseAnalyticEventFactory
    {
        [Preserve]
        public ZitgaAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticServices) : base(signalBus, analyticServices)
        {
        }

        public override AnalyticsEventCustomizationConfig AppsFlyerAnalyticsEventCustomizationConfig { get; set; } = new()
        {
            IncludeEvents = new HashSet<string>()
            {
                "af_first_open",
                "af_ad_complete"
            },

            CustomEventKeys = new Dictionary<string, string>()
            {
                { nameof(FirstOpenEvent), "af_first_open" },
                { nameof(AdsRevenueEvent), "af_ad_complete" }
            }
        };

        public override AnalyticsEventCustomizationConfig FireBaseAnalyticsEventCustomizationConfig { get; set; } = new()
        {
            IgnoreEvents = new HashSet<Type>()
            {
                typeof(GameStarted),
                typeof(FirstOpenEvent),
            },
            CustomEventKeys = new Dictionary<string, string>()
            {
                { nameof(AdsRevenueEvent), "ad_revenue_report" }
            }
        };
    }
}

#endif