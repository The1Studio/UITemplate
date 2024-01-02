namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using ServiceImplementation.Configs;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using Zenject;

    public class CollapsibleBannerRefreshOnScreenChange : IInitializable
    {
        #region Inject

        private readonly SignalBus                  signalBus;
        private readonly UITemplateAdServiceWrapper adServiceWrapper;
        private readonly ThirdPartiesConfig         thirdPartiesConfig;

        #endregion

        public CollapsibleBannerRefreshOnScreenChange(SignalBus signalBus, UITemplateAdServiceWrapper adServiceWrapper, ThirdPartiesConfig thirdPartiesConfig)
        {
            this.signalBus          = signalBus;
            this.adServiceWrapper   = adServiceWrapper;
            this.thirdPartiesConfig = thirdPartiesConfig;
        }

        public void Initialize()
        {
#if ADMOB
            this.signalBus.Subscribe<ScreenShowSignal>(this.OnScreenShow);
#endif
        }

        private void OnScreenShow()
        {
            if (!this.thirdPartiesConfig.AdSettings.AdMob.CollapsibleRefreshOnScreenShow) return;
            this.adServiceWrapper.ShowCollapsibleBannerAd();
        }
    }
}