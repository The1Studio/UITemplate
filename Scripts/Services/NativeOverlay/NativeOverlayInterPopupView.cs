namespace TheOneStudio.UITemplate.UITemplate.Services.NativeOverlay
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using UnityEngine.UI;

    public class NativeOverlayInterPopupView : BaseView
    {
        public Button BtnClose;
    }

    [PopupInfo(nameof(NativeOverlayInterPopupView), isOverlay: true)]
    public class NativeOverlayInterPopupPresenter : BasePopupPresenter<NativeOverlayInterPopupView>
    {
        private readonly UITemplateAdServiceWrapper adServiceWrapper;

        public NativeOverlayInterPopupPresenter(
            SignalBus                  signalBus,
            ILogService                logger,
            UITemplateAdServiceWrapper adServiceWrapper
        ) : base(signalBus, logger)
        {
            this.adServiceWrapper = adServiceWrapper;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.BtnClose.onClick.AddListener(this.CloseView);
        }

        public override UniTask BindData()
        {
            this.adServiceWrapper.ShowNativeOverlayAd();
            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            this.adServiceWrapper.HideNativeOverlayAd();
            base.Dispose();
        }
    }
}