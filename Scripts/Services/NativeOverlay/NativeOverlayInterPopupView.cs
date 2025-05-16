namespace TheOneStudio.UITemplate.UITemplate.Services.NativeOverlay
{
    using System;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Signals;
    using ServiceImplementation.AdsServices.AdMob;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;

    public class NativeOverlayInterModel
    {
        public string InterPlacement;
        public Action OnClose;

        public NativeOverlayInterModel(string interPlacement, Action onClose)
        {
            this.InterPlacement = interPlacement;
            this.OnClose        = onClose;
        }
    }

    public class NativeOverlayInterPopupView : BaseView
    {
    }

    [PopupInfo(nameof(NativeOverlayInterPopupView), false, false, true)]
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

        public override UniTask BindData()
        {
            this.adServiceWrapper.ShowNativeOverlayAd(AdViewPosition.Center);
            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            base.Dispose();
            this.adServiceWrapper.HideNativeOverlayAd();
        }
    }
}