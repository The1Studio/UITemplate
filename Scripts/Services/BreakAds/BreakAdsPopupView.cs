namespace TheOneStudio.UITemplate.UITemplate.Services.BreakAds
{
    using Core.AdsServices.Signals;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using Zenject;

    public class BreakAdsPopupView : BaseView
    {
    }

    [PopupInfo(nameof(BreakAdsPopupView))]
    public class BreakAdsPopupPresenter : BasePopupPresenter<BreakAdsPopupView>
    {
        private readonly SignalBus signalBus;

        public BreakAdsPopupPresenter(SignalBus signalBus) : base(signalBus) { this.signalBus = signalBus; }

        public override UniTask BindData()
        {
            this.signalBus.Subscribe<InterstitialAdClosedSignal>(this.CloseView);
            this.signalBus.Subscribe<InterstitialAdDisplayedFailedSignal>(this.CloseView);
            this.signalBus.Subscribe<InterstitialAdDisplayedSignal>(this.CloseView);

            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            this.signalBus.Unsubscribe<InterstitialAdClosedSignal>(this.CloseView);
            this.signalBus.Unsubscribe<InterstitialAdDisplayedFailedSignal>(this.CloseView);
            this.signalBus.Unsubscribe<InterstitialAdDisplayedSignal>(this.CloseView);
        }
    }
}