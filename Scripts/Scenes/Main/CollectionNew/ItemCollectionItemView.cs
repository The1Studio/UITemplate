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
        private readonly UITemplateInventoryDataController uiTemplateInventoryDataController;

        public ItemCollectionItemPresenter(IGameAssets gameAssets, UITemplateInventoryDataController uiTemplateInventoryDataController) : base(gameAssets)
        {
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
        }

        public override async void BindData(ItemCollectionItemModel param)
        {
            this.View.imgIcon.sprite = await this.GameAssets.LoadAssetAsync<Sprite>(param.ItemBlueprintRecord.ImageAddress);
            this.View.txtPrice.text  = $"{param.ShopBlueprintRecord.Price}";

            this.View.OnBuyAds         = () => param.OnBuyItem?.Invoke(param);
            this.View.OnBuyCoin        = () => param.OnBuyItem?.Invoke(param);
            this.View.OnBuyIap         = () => param.OnBuyItem?.Invoke(param);
            this.View.OnSelect         = () => param.OnSelectItem?.Invoke(param);
            this.View.OnUse            = () => param.OnUseItem?.Invoke(param);
            this.View.OnBuyDailyReward = () => param.OnBuyItem?.Invoke(param);
            this.View.OnBuyLuckySpin   = () => param.OnBuyItem?.Invoke(param);
            this.View.OnBuyStartPack   = () => param.OnBuyItem?.Invoke(param);
            this.SetButtonStatusAndBorderStatus(param);
        }

        private void SetButtonStatusAndBorderStatus(ItemCollectionItemModel param)
        {
            var isCoin      = param.ShopBlueprintRecord.UnlockType == UITemplateItemData.UnlockType.SoftCurrency;
            var isAds       = param.ShopBlueprintRecord.UnlockType == UITemplateItemData.UnlockType.Ads;
            var isIap       = param.ShopBlueprintRecord.UnlockType == UITemplateItemData.UnlockType.IAP;
            var isDaily     = param.ShopBlueprintRecord.UnlockType == UITemplateItemData.UnlockType.DailyReward;
            var isLuckySpin = param.ShopBlueprintRecord.UnlockType == UITemplateItemData.UnlockType.LuckySpin;
            var isStartPack = param.ShopBlueprintRecord.UnlockType == UITemplateItemData.UnlockType.StartedPack;

            var isOwner    = param.ItemData.CurrentStatus == UITemplateItemData.Status.Owned;
            var isUnlocked = param.ItemData.CurrentStatus == UITemplateItemData.Status.Unlocked;
            var isLocked   = param.ItemData.CurrentStatus == UITemplateItemData.Status.Locked;

            var isChoose = param.ItemIndex == param.IndexItemSelected;
            var isUse    = param.ItemIndex == param.IndexItemUsed;

            this.View.btnBuyCoin.gameObject.SetActive(isCoin && !isOwner && isUnlocked);
            this.View.imgLockBuyCoin.gameObject.SetActive(!this.IsItemBuyCoinAble(param));
            this.View.btnUse.gameObject.SetActive(isOwner && !isUse);
            this.View.btnSelect.gameObject.SetActive(!isLocked);
            this.View.btnBuyAds.gameObject.SetActive(isAds && !isOwner && isUnlocked);
            this.View.btnBuyIap.gameObject.SetActive(isIap && !isOwner && isUnlocked);
            this.View.btnDailyReward.gameObject.SetActive(isDaily && !isOwner && isUnlocked);
            this.View.btnLuckySpin.gameObject.SetActive(isLuckySpin && !isOwner && isUnlocked);
            this.View.btnStartPack.gameObject.SetActive(isStartPack && !isOwner && isUnlocked);

            this.View.objUsed.SetActive(param.ItemIndex == param.IndexItemUsed);
            this.View.objChoose.SetActive((!isStartPack || isStartPack && isOwner) && param.ItemIndex == param.IndexItemSelected && param.ItemIndex != param.IndexItemUsed);
            this.View.objChooseStaredPack.SetActive(isStartPack && !isOwner && param.ItemIndex == param.IndexItemSelected && param.ItemIndex != param.IndexItemUsed);
            this.View.objNormal.SetActive(param.ItemIndex != param.IndexItemSelected && (!isStartPack || isStartPack && isOwner));
            this.View.objStaredPack.SetActive(isStartPack && !isUse && !isOwner);
        }

        private bool IsItemBuyCoinAble(ItemCollectionItemModel param)
        {
            if (param.ShopBlueprintRecord.CurrencyID.IsNullOrEmpty())
            {
                return true;
            }

            var currentCoin = this.uiTemplateInventoryDataController.GetCurrencyValue(param.ShopBlueprintRecord.CurrencyID);

            return currentCoin >= param.ShopBlueprintRecord.Price;
        }
    }
}