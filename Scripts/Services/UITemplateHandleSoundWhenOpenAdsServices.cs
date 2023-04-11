namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using Core.AdsServices;
    using Core.AdsServices.Signals;
    using GameFoundation.Scripts.Utilities;
    using Zenject;

    public class UITemplateHandleSoundWhenOpenAdsServices
    {
        private readonly IAudioManager audioManager;

        public UITemplateHandleSoundWhenOpenAdsServices(SignalBus signalBus, IAdServices adServices, IAudioManager audioManager)
        {
            this.audioManager = audioManager;

            signalBus.Subscribe<AppOpenFullScreenContentClosedSignal>(this.OnAppFullScreenContentClosed);
            signalBus.Subscribe<AppOpenFullScreenContentOpenedSignal>(this.OnAppFullScreenContentOpened);

            signalBus.Subscribe<RewardedAdDisplayedSignal>(this.OnDisplayRewardedAd);
            signalBus.Subscribe<RewardedAdCompletedSignal>(this.OnRewardedAdEnded);
            signalBus.Subscribe<RewardedSkippedSignal>(this.OnRewardedAdEnded);

            signalBus.Subscribe<RewardedInterstitialAdCompletedSignal>(this.OnRewardInterDisplay);
            signalBus.Subscribe<RewardInterstitialAdSkippedSignal>(this.OnRewardInterClose);

            signalBus.Subscribe<InterstitialAdDisplayedSignal>(this.OnInterstitialAdDisplayed);
            signalBus.Subscribe<InterstitialAdClosedSignal>(this.OnInterstitialAdClosed);
        }

        private void OnRewardInterClose(RewardInterstitialAdSkippedSignal obj) { this.ResumeSound(); }

        private void OnRewardedAdEnded()                                                    { this.ResumeSound(); }
        private void OnAppFullScreenContentClosed(AppOpenFullScreenContentClosedSignal obj) { this.ResumeSound(); }
        private void OnInterstitialAdClosed(InterstitialAdClosedSignal                 obj) { this.ResumeSound(); }

        private void OnRewardInterDisplay(RewardedInterstitialAdCompletedSignal obj) { this.PauseSound(); }

        private void OnAppFullScreenContentOpened(AppOpenFullScreenContentOpenedSignal obj) { this.PauseSound(); }

        private void OnInterstitialAdDisplayed(InterstitialAdDisplayedSignal obj) { this.PauseSound(); }

        private void OnDisplayRewardedAd(RewardedAdDisplayedSignal obj) { this.PauseSound(); }

        private void PauseSound() { this.audioManager.PauseEverything(); }

        private void ResumeSound() { this.audioManager.ResumeEverything(); }
    }
}