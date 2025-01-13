#if SUPERSONIC
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Supersonic
{
    using System;
    using System.Collections.Generic;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using GameFoundation.DI;
    using GameFoundation.Signals;
    using SupersonicWisdomSDK;
    using UnityEngine.Scripting;

    public class SupersonicAnalyticEventFactory : BaseAnalyticEventFactory, IInitializable
    {
        [Preserve]
        public SupersonicAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticServices)
            : base(signalBus, analyticServices)
        {
        }

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
                { nameof(AdsRevenueEvent), "ad_impression_abi" },
                { "AdsRevenueSourceId", "ad_platform" },
                { "AdNetwork", "ad_source" },
                { "AdUnit", "ad_unit_name" },
                { "AdFormat", "ad_format" },
                { "Placement", "placement" },
                { "Currency", "currency" },
                { "Revenue", "value" },
                { "Message", "errormsg" }
            }
        };

        public void Initialize()
        {
            SupersonicWisdom.Api.Initialize();
            SupersonicWisdom.Api.AddOnReadyListener(this.OnSupersonicWisdomReady);
        }

        public void OnSupersonicWisdomReady() { }

        public override IEvent LevelStart(int level, int gold)
        {
            SupersonicWisdom.Api.NotifyLevelStarted(ESwLevelType.Regular, level, null);
            return base.LevelStart(level, gold);
        }

        public override IEvent LevelLose(int level, int timeSpent, int loseCount)
        {
            SupersonicWisdom.Api.NotifyLevelFailed(ESwLevelType.Regular, level, null);
            return base.LevelLose(level, timeSpent, loseCount);
        }

        public override IEvent LevelWin(int level, int timeSpent, int winCount)
        {
            SupersonicWisdom.Api.NotifyLevelCompleted(ESwLevelType.Regular, level, null);
            return base.LevelWin(level, timeSpent, winCount);
        }
    }
}
#endif