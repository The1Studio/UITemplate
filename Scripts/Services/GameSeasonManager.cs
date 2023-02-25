namespace TheOneStudio.UITemplate.UITemplate.Scripts.Services
{
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices;
    using Zenject;

    public class GameSeasonManager : IInitializable
    {
        private readonly SignalBus        signalBus;
        private readonly UITemplateAdServiceWrapper uiTemplateAdServiceWrapper;
        private readonly ILogService      logService;

        public GameSeasonManager(SignalBus signalBus, UITemplateAdServiceWrapper uiTemplateAdServiceWrapper, ILogService logService)
        {
            this.signalBus        = signalBus;
            this.uiTemplateAdServiceWrapper = uiTemplateAdServiceWrapper;
            this.logService       = logService;
        }

        public void Initialize()
        {
        }
    }
}