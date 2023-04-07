namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using Core.AdsServices;
    using Core.AdsServices.Signals;
    using GameFoundation.Scripts.Utilities;
    using Zenject;

    public class UITemplateHandleSoundWhenOpenAdsServices
    {
        private readonly SignalBus     signalBus;
        private readonly IAdServices   adServices;
        private readonly IAudioManager audioManager;

        public UITemplateHandleSoundWhenOpenAdsServices(SignalBus signalBus, IAdServices adServices, IAudioManager audioManager)
        {
            this.signalBus    = signalBus;
            this.adServices   = adServices;
            this.audioManager = audioManager;
            
          
            this.signalBus.Subscribe<AppOpenFullScreenContentClosedSignal>(this.OnAppFullScreenContentClosed);
            this.signalBus.Subscribe<AppOpenFullScreenContentOpenedSignal>(this.OnAppFullScreenContentOpened);
            
            this.signalBus.Subscribe<RewardedAdDisplayedSignal>(this.OnDisplayRewardedAd);
            this.adServices.RewardedAdCompleted             += this.OnRewardedAdCompleted;
            this.adServices.RewardedAdSkipped               += this.OnRewardedAdSkipped;
            
            this.signalBus.Subscribe<RewardedInterstitialAdDisplayedSignal>(this.OnRewardInterDisplay);
            this.adServices.RewardedInterstitialAdSkipped   += this.OnRewardedInterstitialAdSkipped;
            this.adServices.RewardedInterstitialAdCompleted += this.OnRewardedInterstitialAdCompleted;
            
            this.signalBus.Subscribe<InterstitialAdDisplayedSignal>(this.OnInterstitialAdDisplayed);
            this.adServices.InterstitialAdCompleted         += this.OnInterstitialAdCompleted;
        }

        private void OnRewardInterDisplay(RewardedInterstitialAdDisplayedSignal obj) { this.audioManager.PauseEverything(); }

        private void OnRewardedInterstitialAdCompleted(InterstitialAdNetwork arg1, string arg2) { this.audioManager.ResumeEverything(); }

        private void OnRewardedAdCompleted(RewardedAdNetwork arg1, string arg2) { this.audioManager.ResumeEverything(); }

        private void OnAppFullScreenContentOpened(AppOpenFullScreenContentOpenedSignal obj) { this.audioManager.PauseEverything(); }

        private void OnAppFullScreenContentClosed(AppOpenFullScreenContentClosedSignal obj) { this.audioManager.ResumeEverything(); }

        private void OnInterstitialAdCompleted(InterstitialAdNetwork arg1, string arg2) { this.audioManager.ResumeEverything(); }

        private void OnRewardedInterstitialAdSkipped(InterstitialAdNetwork arg1, string arg2) { this.audioManager.ResumeEverything(); }

        private void OnRewardedAdSkipped(RewardedAdNetwork arg1, string arg2) { this.audioManager.ResumeEverything(); }

        private void OnInterstitialAdDisplayed(InterstitialAdDisplayedSignal obj) { this.audioManager.PauseEverything(); }

        private void OnDisplayRewardedAd(RewardedAdDisplayedSignal obj) { this.audioManager.PauseEverything(); }
    }
}