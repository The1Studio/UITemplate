namespace TheOneStudio.UITemplate.UITemplate.Scripts.Services
{
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices;
    using Zenject;

    public class GameSeasonManager : IInitializable
    {
        private readonly SignalBus        signalBus;
        private readonly AdServiceWrapper adServiceWrapper;
        private readonly ILogService      logService;

        public GameSeasonManager(SignalBus signalBus, AdServiceWrapper adServiceWrapper, ILogService logService)
        {
            this.signalBus        = signalBus;
            this.adServiceWrapper = adServiceWrapper;
            this.logService       = logService;
        }

        public void Initialize()
        {
        }
    }
}