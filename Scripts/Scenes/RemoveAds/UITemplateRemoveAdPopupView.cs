namespace TheOneStudio.UITemplate.UITemplate.Scenes.RemoveAdsBottomBar
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using TheOneStudio.UITemplate.UITemplate.Configs.GameEvents;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TMPro;
    using UnityEngine.UI;
    using Zenject;

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

        public UITemplateRemoveAdPopupPresenter(
            SignalBus                  signalBus,
            UITemplateIapServices      uiTemplateIapServices,
            GameFeaturesSetting        gameFeaturesSetting,
            UITemplateAdServiceWrapper adServiceWrapper
        ) : base(signalBus)
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