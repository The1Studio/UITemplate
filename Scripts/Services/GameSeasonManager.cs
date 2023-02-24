namespace TheOneStudio.UITemplate.UITemplate.Scripts.Services
{
    using GameFoundation.Scripts.Utilities.ApplicationServices;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices;
    using Zenject;

    public class GameSeasonManager
    {
        private readonly SignalBus        signalBus;
        private readonly AdServiceWrapper adServiceWrapper;

        public GameSeasonManager(SignalBus signalBus, AdServiceWrapper adServiceWrapper)
        {
            this.signalBus        = signalBus;
            this.adServiceWrapper = adServiceWrapper;
            this.signalBus.Subscribe<ApplicationPauseSignal>(this.OnAppPause);
        }

        private void OnAppPause(ApplicationPauseSignal obj)
        {
            if (!obj.PauseStatus) return;
            this.adServiceWrapper.ShowAppOpenAd();
        }
    }
}