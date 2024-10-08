namespace TheOneStudio.UITemplate.UITemplate.Services.BreakAds
{
    using System;
    using System.Threading;
    using Core.AdsServices.Signals;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Signals;
    using UnityEngine.Scripting;

    // Rebind this class to your own view
    public class BreakAdsViewHelper
    {
        protected BreakAdsPopupView       View;
        protected BreakAdsPopupPresenter  BreakAdsPopupPresenter;
        protected CancellationTokenSource Cts;

        protected readonly SignalBus SignalBus;


        [Preserve]
        public BreakAdsViewHelper(SignalBus signalBus) { this.SignalBus = signalBus; }

        public virtual void OnViewReady(BreakAdsPopupView view, BreakAdsPopupPresenter breakAdsPopupPresenter)
        {
            this.View                   = view;
            this.BreakAdsPopupPresenter = breakAdsPopupPresenter;
        }

        public virtual async UniTask BindData()
        {
            this.SignalBus.TrySubscribe<InterstitialAdClosedSignal>(this.BreakAdsPopupPresenter.CloseView);
            this.SignalBus.TrySubscribe<InterstitialAdDisplayedFailedSignal>(this.BreakAdsPopupPresenter.CloseView);
            this.SignalBus.TrySubscribe<InterstitialAdDisplayedSignal>(this.BreakAdsPopupPresenter.CloseView);
            this.AutomaticCloseView();
        }

        private async void AutomaticCloseView()
        {
            try
            {
                this.Cts?.Cancel();
                this.Cts = new CancellationTokenSource();
                await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: this.Cts.Token);
                this.BreakAdsPopupPresenter.CloseView();
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        public virtual void Dispose()
        {
            this.SignalBus.TryUnsubscribe<InterstitialAdClosedSignal>(this.BreakAdsPopupPresenter.CloseView);
            this.SignalBus.TryUnsubscribe<InterstitialAdDisplayedFailedSignal>(this.BreakAdsPopupPresenter.CloseView);
            this.SignalBus.TryUnsubscribe<InterstitialAdDisplayedSignal>(this.BreakAdsPopupPresenter.CloseView);
            this.Cts?.Cancel();
        }
    }
}