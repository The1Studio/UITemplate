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

    [PopupInfo(nameof(BreakAdsPopupView), isOverlay: true)]
    public class BreakAdsPopupPresenter : BasePopupPresenter<BreakAdsPopupView>
    {
        private readonly BreakAdsViewHelper breakAdsViewHelper;

        public BreakAdsPopupPresenter(SignalBus signalBus, BreakAdsViewHelper breakAdsViewHelper) : base(signalBus) { this.breakAdsViewHelper = breakAdsViewHelper; }

        protected override void OnViewReady() { this.breakAdsViewHelper.OnViewReady(this.View, this); }

        public override UniTask BindData() { return this.breakAdsViewHelper.BindData(); }

        public override void Dispose() { this.breakAdsViewHelper.Dispose(); }
    }
}