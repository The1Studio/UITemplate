namespace TheOneStudio.UITemplate.UITemplate.Services.BreakAds
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Signals;
    using UnityEngine.Scripting;

    public class BreakAdsPopupView : BaseView
    {
    }

    [PopupInfo(nameof(BreakAdsPopupView), isOverlay: true)]
    public class BreakAdsPopupPresenter : BasePopupPresenter<BreakAdsPopupView>
    {
        private readonly BreakAdsViewHelper breakAdsViewHelper;

        [Preserve]
        public BreakAdsPopupPresenter(SignalBus signalBus, ILogService logger, BreakAdsViewHelper breakAdsViewHelper) : base(signalBus, logger)
        {
            this.breakAdsViewHelper = breakAdsViewHelper;
        }

        protected override void OnViewReady() { this.breakAdsViewHelper.OnViewReady(this.View, this); }

        public override UniTask BindData() { return this.breakAdsViewHelper.BindData(); }

        public override void Dispose() { this.breakAdsViewHelper.Dispose(); }
    }
}