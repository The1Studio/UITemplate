namespace TheOneStudio.UITemplate.UITemplate.Scenes.Popups
{
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using UnityEngine;
    using UnityEngine.Scripting;
    using UnityEngine.UI;

    public class UITemplateItemUnlockPopupModel
    {
        public UITemplateItemUnlockPopupModel(string itemId)
        {
            this.ItemId = itemId;
        }

        public string ItemId { get; set; }
    }

    public class UITemplateItemUnlockPopupView : BaseView
    {
        [SerializeField] private UITemplateCurrencyView currencyView;
        [SerializeField] private Image                  imgItem;
        [SerializeField] private Button                 btnHome;
        [SerializeField] private Button                 btnSkip;
        [SerializeField] private UITemplateAdsButton    btnGet;

        public UITemplateCurrencyView CurrencyView => this.currencyView;
        public Image                  ImgItem      => this.imgItem;
        public Button                 BtnHome      => this.btnHome;
        public UITemplateAdsButton    BtnGet       => this.btnGet;
        public Button                 BtnSkip      => this.btnSkip;
    }

    [PopupInfo(nameof(UITemplateItemUnlockPopupView))]
    public class UITemplateItemUnlockPopupPresenter : UITemplateBasePopupPresenter<UITemplateItemUnlockPopupView, UITemplateItemUnlockPopupModel>
    {
        #region Inject

        protected readonly IScreenManager                    screenManager;
        protected readonly IGameAssets                       gameAssets;
        protected readonly UITemplateAdServiceWrapper        adService;
        protected readonly UITemplateItemBlueprint           itemBlueprint;
        protected readonly UITemplateInventoryDataController inventoryDataController;

        [Preserve]
        public UITemplateItemUnlockPopupPresenter(
            SignalBus signalBus,
            ILogService logService,
            IScreenManager screenManager,
            IGameAssets gameAssets,
            UITemplateAdServiceWrapper adService,
            UITemplateItemBlueprint itemBlueprint,
            UITemplateInventoryDataController inventoryDataController
        ) : base(signalBus, logService)
        {
            this.screenManager           = screenManager;
            this.gameAssets              = gameAssets;
            this.adService               = adService;
            this.itemBlueprint           = itemBlueprint;
            this.inventoryDataController = inventoryDataController;
        }

        #endregion

        protected virtual string AdPlacement => "unlock";

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.BtnGet.OnViewReady(this.adService);
            this.InitButtonListener();
        }

        public override async UniTask BindData(UITemplateItemUnlockPopupModel popupModel)
        {
            this.View.BtnGet.BindData(this.AdPlacement);
            var itemImageAddress = this.itemBlueprint.Values.First(record => record.Id.Equals(popupModel.ItemId)).ImageAddress;
            var itemSprite       = await this.gameAssets.LoadAssetAsync<Sprite>(itemImageAddress);
            this.View.ImgItem.sprite = itemSprite;
        }

        public override void Dispose()
        {
            this.View.BtnGet.Dispose();
        }

        protected virtual void OnClickHome()
        {
            this.screenManager.OpenScreen<UITemplateHomeSimpleScreenPresenter>();
        }

        protected virtual void OnClickGet()
        {
            if (!this.adService.IsRewardedAdReady("")) return;
            this.adService.ShowRewardedAd("", () =>
            {
                this.inventoryDataController.UpdateStatusItemData(this.Model.ItemId, UITemplateItemData.Status.Owned);
                this.CloseView();
            });
        }

        protected virtual void OnClickSkip()
        {
            this.CloseView();
        }

        private void InitButtonListener()
        {
            this.View.BtnHome.onClick.AddListener(this.OnClickHome);
            this.View.BtnGet.onClick.AddListener(this.OnClickGet);
            this.View.BtnSkip.onClick.AddListener(this.OnClickSkip);
        }
    }
}