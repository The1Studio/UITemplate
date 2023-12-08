namespace TheOneStudio.UITemplate.UITemplate.Services.BreakAds
{
    using Core.AdsServices.Signals;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using ServiceImplementation.Configs;
    using UnityEngine;
    using Zenject;

    public class BreakAdsServices : IInitializable, ITickable
    {
        #region inject

        protected readonly SignalBus          SignalBus;
        protected readonly IScreenManager     ScreenManager;
        protected readonly ThirdPartiesConfig ThirdPartiesConfig;
        
        #endregion

        private float BreakAdsOnScreenTime;
        
        public BreakAdsServices(SignalBus signalBus, IScreenManager screenManager, ThirdPartiesConfig thirdPartiesConfig)
        {
            this.SignalBus          = signalBus;
            this.ScreenManager      = screenManager;
            this.ThirdPartiesConfig = thirdPartiesConfig;
        }

        public void Initialize()
        {
            if (this.ThirdPartiesConfig.AdSettings.EnableBreakAds)
                this.SignalBus.Subscribe<InterstitialAdCalledSignal>(this.OpenBreakAds);
        }

        protected virtual void OpenBreakAds()
        {
            this.BreakAdsOnScreenTime = 0;
            this.ScreenManager.OpenScreen<BreakAdsPopupPresenter>().Forget();
        }
        
        public async void Tick()
        {
            while (this.ScreenManager.CurrentActiveScreen.Value is BreakAdsPopupPresenter)
            {
                this.BreakAdsOnScreenTime += Time.unscaledDeltaTime;
                if (this.BreakAdsOnScreenTime >= 2f)
                {
                    await this.ScreenManager.CloseCurrentScreen();
                }
            }
        }
    }
}