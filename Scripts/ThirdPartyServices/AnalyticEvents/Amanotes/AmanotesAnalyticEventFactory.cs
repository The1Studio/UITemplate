#if AMANOTES
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Amanotes
{
    using System;
    using System.Collections.Generic;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents;
    using Zenject;

    public class AmanotesAnalyticEventFactory : BaseAnalyticEventFactory
    {
        public AmanotesAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticServices) : base(signalBus, analyticServices) { }

        public override AnalyticsEventCustomizationConfig AppsFlyerAnalyticsEventCustomizationConfig { get; set; } = new()
        {
            IgnoreAllEvents = true
        };

        public override AnalyticsEventCustomizationConfig FireBaseAnalyticsEventCustomizationConfig { get; set; } = new()
        {
            IgnoreEvents    = new HashSet<Type>(),
            CustomEventKeys = new Dictionary<string, string>()
        };
    }
}
#endif