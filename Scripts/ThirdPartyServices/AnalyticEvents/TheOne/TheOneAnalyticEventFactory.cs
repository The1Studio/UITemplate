#if THEONE
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.TheOne
{
    using System;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents;
    using UnityEngine.Scripting;

    public class TheOneAnalyticEventFactory : BaseAnalyticEventFactory
    {
        [Preserve] public TheOneAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticServices) : base(signalBus, analyticServices) { }

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

        public override IEvent LevelLose(int level, int timeSpent, int loseCount)
        {
            this.analyticServices.Track(new LevelEnd(level, "win", 0, timeSpent, DateTimeOffset.UtcNow.ToUnixTimeSeconds()));
            return base.LevelLose(level, timeSpent, loseCount);
        }


        public override IEvent LevelWin(int level, int timeSpent, int winCount)
        {
            this.analyticServices.Track(new LevelEnd(level, "lose", 0, timeSpent, DateTimeOffset.UtcNow.ToUnixTimeSeconds()));
            return base.LevelWin(level, timeSpent, winCount);
        }
    }
}
#endif