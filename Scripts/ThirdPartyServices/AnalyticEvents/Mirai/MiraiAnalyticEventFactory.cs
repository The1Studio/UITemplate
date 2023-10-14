#if MIRAI
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Mirai
{
    using Core.AnalyticServices;
    using Zenject;

    public class MiraiAnalyticEventFactory : BaseAnalyticEventFactory
    {
        public MiraiAnalyticEventFactory(SignalBus signalBus, IAnalyticServices analyticServices) : base(signalBus, analyticServices)
        {
        }
    }
}
#endif