#if THEONE
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.TheOne
{
    using System;
    using System.Collections.Generic;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine.Scripting;

    public class TheOneAnalyticEventFactory : BaseAnalyticEventFactory
    {
        [Preserve] public TheOneAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticServices, UITemplateLevelDataController levelDataController) : base(signalBus, analyticServices ,levelDataController) { }

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

        private string playMode => "classic";
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

        public override IEvent LevelLose(int level, int timeSpent, int loseCount, Dictionary<string, object> metadata = null) { return new LevelEnd(level, false, this.playMode, "lose", 0, timeSpent, 0, DateTimeOffset.UtcNow.ToUnixTimeSeconds()); }

        public override IEvent LevelWin(int   level, int timeSpent, int winCount, Dictionary<string, object> metadata = null) { return new LevelEnd(level, true, this.playMode, "win", 0, timeSpent, 0, DateTimeOffset.UtcNow.ToUnixTimeSeconds()); }
        public override IEvent LevelStart(int level, int gold, Dictionary<string, object> metadata = null) { return new LevelStart(level, gold); }
    }
}

#endif