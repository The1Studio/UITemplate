namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.CollectionNew
{
    using System;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Scripting;
    using UnityEngine.UI;

    public class ItemCollectionItemModel
    {
        public int                  ItemIndex           { get; set; }
        public int                  IndexItemUsed       { get; set; }
        public int                  IndexItemSelected   { get; set; }
        public UITemplateItemRecord ItemBlueprintRecord => this.ItemData.ItemBlueprintRecord;
        public UITemplateShopRecord ShopBlueprintRecord => this.ItemData.ShopBlueprintRecord;
        public UITemplateItemData   ItemData            { get; set; }

        public Action<ItemCollectionItemModel> OnSelectItem { get; set; }
        public Action<ItemCollectionItemModel> OnBuyItem    { get; set; }
        public Action<ItemCollectionItemModel> OnUseItem    { get; set; }
    }

    public class ItemCollectionItemView : TViewMono
    {
        public GameObject      objChoose, objNormal, objUsed, objStaredPack, objChooseStaredPack;
        public Image           imgIcon,   imgLockBuyCoin;
        public TextMeshProUGUI txtPrice;
        public Button          btnBuyCoin, btnBuyAds, btnBuyIap, btnDailyReward, btnLuckySpin, btnSelect, btnUse, btnStartPack;

        public Action OnBuyCoin, OnBuyAds, OnBuyIap, OnSelect, OnUse, OnBuyDailyReward, OnBuyLuckySpin, OnBuyStartPack;

        private void Awake()
        {
            this.btnBuyCoin.onClick.AddListener(() => { this.OnBuyCoin?.Invoke(); });
            this.btnBuyAds.onClick.AddListener(() => { this.OnBuyAds?.Invoke(); });
            this.btnBuyIap.onClick.AddListener(() => { this.OnBuyIap?.Invoke(); });
            this.btnSelect.onClick.AddListener(() => { this.OnSelect?.Invoke(); });
            this.btnUse.onClick.AddListener(() => { this.OnUse?.Invoke(); });
            this.btnDailyReward.onClick.AddListener(() => { this.OnBuyDailyReward?.Invoke(); });
            this.btnLuckySpin.onClick.AddListener(() => { this.OnBuyLuckySpin?.Invoke(); });
            this.btnStartPack.onClick.AddListener(() => { this.OnBuyStartPack?.Invoke(); });
        }
    }

    public class ItemCollectionItemPresenter : BaseUIItemPresenter<ItemCollectionItemView, ItemCollectionItemModel>
    {
        private readonly UITemplateCollectionItemViewHelper uiTemplateCollectionItemViewHelper;

        [Preserve]
        public ItemCollectionItemPresenter(IGameAssets gameAssets, UITemplateInventoryDataController uiTemplateInventoryDataController,
            UITemplateCollectionItemViewHelper uiTemplateCollectionItemViewHelper) : base(gameAssets)
        {
            this.uiTemplateCollectionItemViewHelper = uiTemplateCollectionItemViewHelper;
        }

        public override async void BindData(ItemCollectionItemModel param)
        {
            this.uiTemplateCollectionItemViewHelper.BindDataItem(param, this.View);
        }

        public override void Dispose()
        {
            this.uiTemplateCollectionItemViewHelper.DisposeItem(this.View);
            base.Dispose();
        }
    }
}