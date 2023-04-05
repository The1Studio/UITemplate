namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.CollectionNew
{
    using System;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models;
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
        public GameObject      objChoose, objNormal, objUsed;
        public Image           imgIcon;
        public TextMeshProUGUI txtPrice;
        public Button          btnBuyCoin, btnBuyAds, btnBuyIap, btnDailyReward, btnLuckySpin, btnSelect, btnUse;

        public Action OnBuyCoin, OnBuyAds, OnBuyIap, OnSelect, OnUse, OnBuyDailyReward, OnBuyLuckySpin;

        private void Awake()
        {
            this.btnBuyCoin.onClick.AddListener(() =>
            {
                this.OnBuyCoin?.Invoke();
            });
            this.btnBuyAds.onClick.AddListener(() =>
            {
                this.OnBuyAds?.Invoke();
            });
            this.btnBuyIap.onClick.AddListener(() =>
            {
                this.OnBuyIap?.Invoke();
            });
            this.btnSelect.onClick.AddListener(() =>
            {
                this.OnSelect?.Invoke();
            });
            this.btnUse.onClick.AddListener(() =>
            {
                this.OnUse?.Invoke();
            });
            this.btnDailyReward.onClick.AddListener(() =>
            {
                this.OnBuyDailyReward?.Invoke();
            });
            this.btnLuckySpin.onClick.AddListener(() =>
            {
                this.OnBuyLuckySpin?.Invoke();
            });
        }
    }

    public class ItemCollectionItemPresenter : BaseUIItemPresenter<ItemCollectionItemView, ItemCollectionItemModel>
    {
        public ItemCollectionItemPresenter(IGameAssets gameAssets) : base(gameAssets)
        {
        }

        public override async void BindData(ItemCollectionItemModel param)
        {
            this.View.imgIcon.sprite = await this.GameAssets.LoadAssetAsync<Sprite>(param.ItemBlueprintRecord.ImageAddress);
            this.View.txtPrice.text  = $"{param.ShopBlueprintRecord.Price}";
            this.View.objUsed.SetActive(param.ItemIndex == param.IndexItemUsed);
            this.View.objChoose.SetActive(param.ItemIndex == param.IndexItemSelected && param.ItemIndex != param.IndexItemUsed);
            this.View.objNormal.SetActive(param.ItemIndex != param.IndexItemSelected);

            this.View.OnBuyAds         = () => param.OnBuyItem?.Invoke(param);
            this.View.OnBuyCoin        = () => param.OnBuyItem?.Invoke(param);
            this.View.OnBuyIap         = () => param.OnBuyItem?.Invoke(param);
            this.View.OnSelect         = () => param.OnSelectItem?.Invoke(param);
            this.View.OnUse            = () => param.OnUseItem?.Invoke(param);
            this.View.OnBuyDailyReward = () => param.OnBuyItem?.Invoke(param);
            this.View.OnBuyLuckySpin   = () => param.OnBuyItem?.Invoke(param);
            this.SetButtonStatus(param);
        }

        private void SetButtonStatus(ItemCollectionItemModel param)
        {
            var isCoin      = param.ShopBlueprintRecord.UnlockType == UITemplateItemData.UnlockType.SoftCurrency;
            var isAds       = param.ShopBlueprintRecord.UnlockType == UITemplateItemData.UnlockType.Ads;
            var isIap       = param.ShopBlueprintRecord.UnlockType == UITemplateItemData.UnlockType.IAP;
            var isDaily     = param.ShopBlueprintRecord.UnlockType == UITemplateItemData.UnlockType.DailyReward;
            var isLuckySpin = param.ShopBlueprintRecord.UnlockType == UITemplateItemData.UnlockType.LuckySpin;

            var isOwner    = param.ItemData.CurrentStatus == UITemplateItemData.Status.Owned;
            var isUnlocked = param.ItemData.CurrentStatus == UITemplateItemData.Status.Unlocked;
            var isLocked   = param.ItemData.CurrentStatus == UITemplateItemData.Status.Locked;

            var isChoose = param.ItemIndex == param.IndexItemSelected;
            var isUse    = param.ItemIndex == param.IndexItemUsed;

            this.View.btnBuyCoin.gameObject.SetActive(isCoin && !isOwner && isUnlocked);
            this.View.btnUse.gameObject.SetActive(isOwner && !isUse);
            this.View.btnSelect.gameObject.SetActive(!isLocked);
            this.View.btnBuyAds.gameObject.SetActive(isAds && !isOwner && isUnlocked);
            this.View.btnBuyIap.gameObject.SetActive(isIap && !isOwner && isUnlocked);
            this.View.btnDailyReward.gameObject.SetActive(isDaily && !isOwner && isUnlocked);
            this.View.btnLuckySpin.gameObject.SetActive(isLuckySpin && !isOwner && isUnlocked);
        }
    }
}