namespace TheOneStudio.UITemplate.UITemplate.Scenes.RemoveAdsBottomBar
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Configs.GameEvents;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TMPro;
    using UnityEngine.Scripting;
    using UnityEngine.UI;

    public class UITemplateRemoveAdPopupView : BaseView
    {
        public TMP_Text priceText;
        public Button   btnRemoveAds;
        public Button   btnClose;
    }

    [PopupInfo(nameof(UITemplateRemoveAdPopupView))]
    public class UITemplateRemoveAdPopupPresenter : UITemplateBasePopupPresenter<UITemplateRemoveAdPopupView>
    {
        protected readonly UITemplateIapServices      uiTemplateIapServices;
        protected readonly GameFeaturesSetting        gameFeaturesSetting;
        protected readonly UITemplateAdServiceWrapper adServiceWrapper;

        [Preserve]
        public UITemplateRemoveAdPopupPresenter(
            SignalBus                  signalBus,
            ILogService                logger,
            UITemplateIapServices      uiTemplateIapServices,
            GameFeaturesSetting        gameFeaturesSetting,
            UITemplateAdServiceWrapper adServiceWrapper
        ) : base(signalBus, logger)
        {
            this.uiTemplateIapServices = uiTemplateIapServices;
            this.gameFeaturesSetting   = gameFeaturesSetting;
            this.adServiceWrapper      = adServiceWrapper;
        }

        private string ProductId => this.gameFeaturesSetting.IAPConfig.removeAdsProductId;

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.btnRemoveAds.onClick.AddListener(this.OnRemoveAdsClicked);
            this.View.btnClose.onClick.AddListener(this.OnClickCloseButton);
        }

        private void OnRemoveAdsClicked()
        {
            this.uiTemplateIapServices.BuyProduct(
                this.View.btnRemoveAds.gameObject, this.ProductId,
                _ =>
                {
                    this.adServiceWrapper.RemoveAds();
                    this.CloseView();
                }
            );
        }

        protected virtual void OnClickCloseButton()
        {
            this.CloseView();
        }

        public override UniTask BindData()
        {
            this.View.priceText.text = this.uiTemplateIapServices.GetPriceById(this.ProductId, "0.99$");

            return UniTask.CompletedTask;
        }
    }
}