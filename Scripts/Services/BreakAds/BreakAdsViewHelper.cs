namespace TheOneStudio.UITemplate.UITemplate.Services.BreakAds
{
    using System;
    using System.Threading;
    using Core.AdsServices.Signals;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Signals;
    using ServiceImplementation.Configs;
    using UnityEngine.Scripting;

    // Rebind this class to your own view
    public class BreakAdsViewHelper
    {
        protected BreakAdsPopupView       View;
        protected BreakAdsPopupPresenter  BreakAdsPopupPresenter;
        protected CancellationTokenSource Cts;
        
        public bool IsViewReady => BreakAdsPopupPresenter.ScreenStatus == ScreenStatus.Opened;
        
        protected readonly SignalBus          SignalBus;
        private readonly   ThirdPartiesConfig thirdPartiesConfig;

        [Preserve]
        public BreakAdsViewHelper(SignalBus signalBus, ThirdPartiesConfig thirdPartiesConfig)
        {
            this.SignalBus          = signalBus;
            this.thirdPartiesConfig = thirdPartiesConfig;
        }

        public virtual void OnViewReady(BreakAdsPopupView view, BreakAdsPopupPresenter breakAdsPopupPresenter)
        {
            this.View                   = view;
            this.BreakAdsPopupPresenter = breakAdsPopupPresenter;
        }

        public virtual async UniTask BindData()
        {
            this.SignalBus.Subscribe<InterstitialAdClosedSignal>(this.BreakAdsPopupPresenter.CloseView);
            this.SignalBus.Subscribe<InterstitialAdDisplayedFailedSignal>(this.BreakAdsPopupPresenter.CloseView);
            this.SignalBus.Subscribe<InterstitialAdDisplayedSignal>(this.BreakAdsPopupPresenter.CloseView);
            this.AutomaticCloseView();
        }

        private async void AutomaticCloseView()
        {
            try
            {
                this.Cts?.Cancel();
                this.Cts = new();
                await UniTask.Delay(TimeSpan.FromSeconds(this.thirdPartiesConfig.AdSettings.TimeDelayCloseBreakAdsPopup), cancellationToken: this.Cts.Token);
                this.BreakAdsPopupPresenter.CloseView();
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        public virtual void Dispose()
        {
            this.SignalBus.Unsubscribe<InterstitialAdClosedSignal>(this.BreakAdsPopupPresenter.CloseView);
            this.SignalBus.Unsubscribe<InterstitialAdDisplayedFailedSignal>(this.BreakAdsPopupPresenter.CloseView);
            this.SignalBus.Unsubscribe<InterstitialAdDisplayedSignal>(this.BreakAdsPopupPresenter.CloseView);
            this.Cts?.Cancel();
        }
    }
}