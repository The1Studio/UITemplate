namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using Core.AdsServices.Signals;
    using GameFoundation.DI;
    using GameFoundation.Scripts.Utilities;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using UnityEngine.Scripting;

    public class UITemplateHandleSoundWhenOpenAdsServices : IInitializable, IDisposable
    {
        private readonly SignalBus     signalBus;
        private readonly IAudioService audioManager;

        [Preserve]
        public UITemplateHandleSoundWhenOpenAdsServices(SignalBus signalBus, IAudioService audioManager)
        {
            this.signalBus    = signalBus;
            this.audioManager = audioManager;
        }

        public void Initialize()
        {
            this.signalBus.Subscribe<AppOpenFullScreenContentClosedSignal>(this.OnAppFullScreenContentClosed);
            this.signalBus.Subscribe<AppOpenFullScreenContentOpenedSignal>(this.OnAppFullScreenContentOpened);

            this.signalBus.Subscribe<RewardedAdCalledSignal>(this.OnRewardAdsCalled);
            this.signalBus.Subscribe<RewardedAdDisplayedSignal>(this.OnDisplayRewardedAd);
            this.signalBus.Subscribe<RewardedAdClosedSignal>(this.OnRewardedAdClose);
            this.signalBus.Subscribe<RewardedAdShowFailedSignal>(this.OnRewardedAdShowFail);

            this.signalBus.Subscribe<RewardInterstitialAdCalledSignal>(this.OnRewardInterAdsCalled);
            this.signalBus.Subscribe<RewardInterstitialAdClosedSignal>(this.OnRewardInterClosed);

            this.signalBus.Subscribe<InterstitialAdDisplayedSignal>(this.OnInterstitialAdDisplayed);
            this.signalBus.Subscribe<InterstitialAdCalledSignal>(this.OnInterAdCalled);
            this.signalBus.Subscribe<InterstitialAdClosedSignal>(this.OnInterstitialAdClosed);

            this.signalBus.Subscribe<LevelStartedSignal>(this.OnStartLevel);
        }

        public void Dispose()
        {
            this.signalBus.Unsubscribe<AppOpenFullScreenContentClosedSignal>(this.OnAppFullScreenContentClosed);
            this.signalBus.Unsubscribe<AppOpenFullScreenContentOpenedSignal>(this.OnAppFullScreenContentOpened);

            this.signalBus.Unsubscribe<RewardedAdCalledSignal>(this.OnRewardAdsCalled);
            this.signalBus.Unsubscribe<RewardedAdDisplayedSignal>(this.OnDisplayRewardedAd);
            this.signalBus.Unsubscribe<RewardedAdClosedSignal>(this.OnRewardedAdClose);
            this.signalBus.Unsubscribe<RewardedAdShowFailedSignal>(this.OnRewardedAdShowFail);

            this.signalBus.Unsubscribe<RewardInterstitialAdCalledSignal>(this.OnRewardInterAdsCalled);
            this.signalBus.Unsubscribe<RewardInterstitialAdClosedSignal>(this.OnRewardInterClosed);

            this.signalBus.Unsubscribe<InterstitialAdDisplayedSignal>(this.OnInterstitialAdDisplayed);
            this.signalBus.Unsubscribe<InterstitialAdCalledSignal>(this.OnInterAdCalled);
            this.signalBus.Unsubscribe<InterstitialAdClosedSignal>(this.OnInterstitialAdClosed);

            this.signalBus.Unsubscribe<LevelStartedSignal>(this.OnStartLevel);
        }

        private void OnStartLevel(LevelStartedSignal                                   obj) { this.ResumeSound(); }
        private void OnRewardInterClosed(RewardInterstitialAdClosedSignal              obj) { this.ResumeSound(); }
        private void OnRewardedAdClose(RewardedAdClosedSignal                          obj) { this.ResumeSound(); }
        private void OnRewardedAdShowFail(RewardedAdShowFailedSignal                   obj) { this.ResumeSound(); }
        private void OnAppFullScreenContentClosed(AppOpenFullScreenContentClosedSignal obj) { this.ResumeSound(); }
        private void OnInterstitialAdClosed(InterstitialAdClosedSignal                 obj) { this.ResumeSound(); }

        private void OnInterAdCalled(InterstitialAdCalledSignal                        obj) { this.PauseSound(); }
        private void OnRewardAdsCalled(RewardedAdCalledSignal                          obj) { this.PauseSound(); }
        private void OnRewardInterAdsCalled(RewardInterstitialAdCalledSignal           obj) { this.PauseSound(); }
        private void OnAppFullScreenContentOpened(AppOpenFullScreenContentOpenedSignal obj) { this.PauseSound(); }
        private void OnInterstitialAdDisplayed(InterstitialAdDisplayedSignal           obj) { this.PauseSound(); }
        private void OnDisplayRewardedAd(RewardedAdDisplayedSignal                     obj) { this.PauseSound(); }

        private void PauseSound() { this.audioManager.PauseEverything(); }

        private void ResumeSound() { this.audioManager.ResumeEverything(); }
    }
}