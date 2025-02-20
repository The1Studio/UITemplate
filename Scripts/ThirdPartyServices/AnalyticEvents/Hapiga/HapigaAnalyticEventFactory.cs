#if HAPIGA
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Hapiga
{
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine.Scripting;

    public class HapigaAnalyticEventFactory : BaseAnalyticEventFactory
    {
        [Preserve]
        public HapigaAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticServices, UITemplateLevelDataController levelDataController) : base(signalBus, analyticServices, levelDataController)
        {
        }

        public override IEvent LevelStart(int level, int gold)
        {
            if (level < 100)
                this.analyticServices.Track(new CustomEvent()
                {
                    EventName = $"start_level_{level:D3}",
                });
            return base.LevelStart(level, gold);
        }

        public override IEvent LevelWin(int level, int timeSpent, int winCount)
        {
            if (level < 100)
                this.analyticServices.Track(new CustomEvent()
                {
                    EventName = $"win_level_{level:D3}",
                });
            return base.LevelWin(level, timeSpent, winCount);
        }
    }
}
#endif