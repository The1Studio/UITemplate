namespace TheOneStudio.UITemplate.UITemplate.Services.BreakAds
{
    using Core.AdsServices.Signals;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using ServiceImplementation.Configs;
    using Zenject;

    public class BreakAdsServices : IInitializable
    {
        #region inject

        private readonly SignalBus          signalBus;
        private readonly IScreenManager     screenManager;
        private readonly ThirdPartiesConfig thirdPartiesConfig;

        public BreakAdsServices(SignalBus signalBus, IScreenManager screenManager, ThirdPartiesConfig thirdPartiesConfig)
        {
            this.signalBus          = signalBus;
            this.screenManager      = screenManager;
            this.thirdPartiesConfig = thirdPartiesConfig;
        }

        #endregion

        public void Initialize()
        {
            if (this.thirdPartiesConfig.AdSettings.EnableBreakAds)
                this.signalBus.Subscribe<InterstitialAdCalledSignal>(this.OpenBreakAds);
        }

        protected virtual void OpenBreakAds() { this.screenManager.OpenScreen<BreakAdsPopupPresenter>().Forget(); }
    }
}