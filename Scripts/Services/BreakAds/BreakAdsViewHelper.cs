namespace TheOneStudio.UITemplate.UITemplate.Services.BreakAds
{
    using Core.AdsServices.Signals;
    using Cysharp.Threading.Tasks;
    using Zenject;

    // Rebind this class to your own view
    public class BreakAdsViewHelper
    {
        protected BreakAdsPopupView      View;
        protected BreakAdsPopupPresenter BreakAdsPopupPresenter;

        protected readonly SignalBus SignalBus;

        public BreakAdsViewHelper(SignalBus signalBus) { this.SignalBus = signalBus; }

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
        }

        public virtual void Dispose()
        {
            this.SignalBus.Unsubscribe<InterstitialAdClosedSignal>(this.BreakAdsPopupPresenter.CloseView);
            this.SignalBus.Unsubscribe<InterstitialAdDisplayedFailedSignal>(this.BreakAdsPopupPresenter.CloseView);
            this.SignalBus.Unsubscribe<InterstitialAdDisplayedSignal>(this.BreakAdsPopupPresenter.CloseView);
        }
    }
}