namespace TheOneStudio.HyperCasual.DrawCarBase.Scripts.Runtime.Scenes.Building.Popup
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateBuildingNotEnoughCoinPopupView : BaseView
    {
        public UITemplateAdsButton BtnGetGold;
        public Button              BtnClose;
    }

    [PopupInfo(nameof(UITemplateBuildingNotEnoughCoinPopupView), false, true, true)]
    public class UITemplateBuildingNotEnoughCoinPopupPresenter : UITemplateBasePopupPresenter<UITemplateBuildingNotEnoughCoinPopupView>
    {
        protected virtual string placement => "Building";
        protected virtual int    coinAdd   => 300;

        private readonly UITemplateInventoryDataController uiTemplateInventoryDataController;
        private readonly UITemplateAdServiceWrapper        uiTemplateAdServiceWrapper;

        public UITemplateBuildingNotEnoughCoinPopupPresenter(SignalBus signalBus,
            UITemplateInventoryDataController uiTemplateInventoryDataController,
            UITemplateAdServiceWrapper uiTemplateAdServiceWrapper) : base(signalBus)
        {
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
            this.uiTemplateAdServiceWrapper        = uiTemplateAdServiceWrapper;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.BtnGetGold.onClick.AddListener(this.GetMoreGold);
            this.View.BtnClose.onClick.AddListener(this.OnClose);
        }

        public override UniTask BindData() { return default; }

        protected virtual void OnClose() { this.CloseView(); }

        protected virtual void GetMoreGold() { this.uiTemplateAdServiceWrapper.ShowRewardedAd(this.placement, this.GetGoldCompleted); }

        protected virtual async void GetGoldCompleted()
        {
            this.CloseView();
            await this.uiTemplateInventoryDataController.AddCurrency(this.coinAdd, startAnimationRect: this.View.BtnGetGold.transform as RectTransform);
        }
    }
}