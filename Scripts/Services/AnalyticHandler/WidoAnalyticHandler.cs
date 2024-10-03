#if WIDO

namespace TheOneStudio.UITemplate.UITemplate.Services.AnalyticHandler
{
    using System.Collections.Generic;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Signal;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents;
    using UnityEngine.Scripting;

    [Preserve]
    public class WidoAnalyticHandler : UITemplateAnalyticHandler
    {
        public WidoAnalyticHandler(SignalBus                           signalBus,
                                   IAnalyticServices                   analyticServices,
                                   IAnalyticEventFactory               analyticEventFactory,
                                   UITemplateLevelDataController       uiTemplateLevelDataController,
                                   UITemplateInventoryDataController   uITemplateInventoryDataController,
                                   UITemplateDailyRewardController     uiTemplateDailyRewardController,
                                   UITemplateGameSessionDataController uITemplateGameSessionDataController)
            : base(signalBus, analyticServices, analyticEventFactory, uiTemplateLevelDataController, uITemplateInventoryDataController, uiTemplateDailyRewardController, uITemplateGameSessionDataController)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            this.signalBus.Subscribe<AdRevenueSignal>(this.HandleAdRevenue);
        }

        private void HandleAdRevenue(AdRevenueSignal obj)
        {
            var paramDic = new Dictionary<string, object>()
            {
                { "ad_platform", obj.AdsRevenueEvent.AdNetwork },
                { "placement", obj.AdsRevenueEvent.Placement },
            };

            switch (obj.AdsRevenueEvent.AdFormat)
            {
                case "Banner":
                    this.Track(new CustomEvent()
                    {
                        EventName       = "banner_show_success",
                        EventProperties = paramDic,
                    });
                    break;
                case "CollapsibleBanner":
                    this.Track(new CustomEvent()
                    {
                        EventName       = "collap_banner_show_success",
                        EventProperties = paramDic,
                    });
                    break;
                case "MREC":
                    this.Track(new CustomEvent()
                    {
                        EventName       = "mrec_show_success",
                        EventProperties = paramDic,
                    });
                    break;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            this.signalBus.Unsubscribe<AdRevenueSignal>(this.HandleAdRevenue);
        }
    }
}

#endif