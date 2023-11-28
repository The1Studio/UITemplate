namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using Core.AdsServices.Signals;
    using GameFoundation.Scripts.Utilities;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using Zenject;

    public class UITemplateHandleSoundWhenOpenAdsServices
    {
        private readonly IAudioService audioManager;

        public UITemplateHandleSoundWhenOpenAdsServices(SignalBus signalBus, IAudioService audioManager)
        {
            this.audioManager = audioManager;

            signalBus.Subscribe<AppOpenFullScreenContentClosedSignal>(this.OnAppFullScreenContentClosed);
            signalBus.Subscribe<AppOpenFullScreenContentOpenedSignal>(this.OnAppFullScreenContentOpened);

            signalBus.Subscribe<RewardedAdCalledSignal>(this.OnRewardAdsCalled);
            signalBus.Subscribe<RewardedAdDisplayedSignal>(this.OnDisplayRewardedAd);
            signalBus.Subscribe<RewardedAdClosedSignal>(this.OnRewardedAdClose);
            signalBus.Subscribe<RewardedAdShowFailedSignal>(this.OnRewardedAdShowFail);

            signalBus.Subscribe<RewardInterstitialAdCalledSignal>(this.OnRewardInterAdsCalled);
            signalBus.Subscribe<RewardInterstitialAdClosedSignal>(this.OnRewardInterClosed);

            signalBus.Subscribe<InterstitialAdDisplayedSignal>(this.OnInterstitialAdDisplayed);
            signalBus.Subscribe<InterstitialAdCalledSignal>(this.OnInterAdCalled);
            signalBus.Subscribe<InterstitialAdClosedSignal>(this.OnInterstitialAdClosed);

            signalBus.Subscribe<LevelStartedSignal>(this.OnStartLevel);
        }

        private void OnStartLevel(LevelStartedSignal obj)                                   { this.ResumeSound(); }
        private void OnRewardInterClosed(RewardInterstitialAdClosedSignal obj)              { this.ResumeSound(); }
        private void OnRewardedAdClose(RewardedAdClosedSignal obj)                          { this.ResumeSound(); }
        private void OnRewardedAdShowFail(RewardedAdShowFailedSignal obj)                   { this.ResumeSound(); }
        private void OnAppFullScreenContentClosed(AppOpenFullScreenContentClosedSignal obj) { this.ResumeSound(); }
        private void OnInterstitialAdClosed(InterstitialAdClosedSignal obj)                 { this.ResumeSound(); }

        private void OnInterAdCalled(InterstitialAdCalledSignal obj)                        { this.PauseSound(); }
        private void OnRewardAdsCalled(RewardedAdCalledSignal obj)                          { this.PauseSound(); }
        private void OnRewardInterAdsCalled(RewardInterstitialAdCalledSignal obj)           { this.PauseSound(); }
        private void OnAppFullScreenContentOpened(AppOpenFullScreenContentOpenedSignal obj) { this.PauseSound(); }
        private void OnInterstitialAdDisplayed(InterstitialAdDisplayedSignal obj)           { this.PauseSound(); }
        private void OnDisplayRewardedAd(RewardedAdDisplayedSignal obj)                     { this.PauseSound(); }

        private void PauseSound() { this.audioManager.PauseEverything(); }

        private void ResumeSound() { this.audioManager.ResumeEverything(); }
    }
}