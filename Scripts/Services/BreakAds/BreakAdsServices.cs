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

        protected readonly SignalBus          SignalBus;
        protected readonly IScreenManager     ScreenManager;
        protected readonly ThirdPartiesConfig ThirdPartiesConfig;

        public BreakAdsServices(SignalBus signalBus, IScreenManager screenManager, ThirdPartiesConfig thirdPartiesConfig)
        {
            this.SignalBus          = signalBus;
            this.ScreenManager      = screenManager;
            this.ThirdPartiesConfig = thirdPartiesConfig;
        }

        #endregion

        public void Initialize()
        {
            if (this.ThirdPartiesConfig.AdSettings.EnableBreakAds)
                this.SignalBus.Subscribe<InterstitialAdCalledSignal>(this.OpenBreakAds);
        }

        protected virtual void OpenBreakAds() { this.ScreenManager.OpenScreen<BreakAdsPopupPresenter>().Forget(); }
    }
}