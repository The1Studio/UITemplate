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
        public int                  ItemIndex            { get; set; }
        public int                  IndexItemSelected    { get; set; }
        public UITemplateItemRecord UITemplateItemRecord { get; set; }
        public UITemplateItemData   ItemData             { get; set; }

        public Action<ItemCollectionItemModel> OnSelectItem { get; set; }
        public Action<ItemCollectionItemModel> OnBuyItem    { get; set; }
    }

    public class ItemCollectionItemView : TViewMono
    {
        public GameObject      objChoose, objNormal;
        public Image           imgIcon;
        public TextMeshProUGUI txtPrice;
        public Button          btnBuyCoin, btnBuyAds, btnBuyIap, btnSelect;

        public Action OnBuyCoin, OnBuyAds, OnBuyIap, OnSelect;

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
        }
    }

    public class ItemCollectionItemPresenter : BaseUIItemPresenter<ItemCollectionItemView, ItemCollectionItemModel>
    {
        public ItemCollectionItemPresenter(IGameAssets gameAssets) : base(gameAssets)
        {
        }

        public override async void BindData(ItemCollectionItemModel param)
        {
            this.View.imgIcon.sprite = await this.GameAssets.LoadAssetAsync<Sprite>(param.UITemplateItemRecord.ImageAddress);
            this.View.txtPrice.text  = $"{param.UITemplateItemRecord.Price}";
            this.View.objChoose.SetActive(param.ItemIndex == param.IndexItemSelected);
            this.View.objNormal.SetActive(param.ItemIndex != param.IndexItemSelected);

            this.View.OnBuyAds  = () => param.OnBuyItem?.Invoke(param);
            this.View.OnBuyCoin = () => param.OnBuyItem?.Invoke(param);
            this.View.OnBuyIap  = () => param.OnBuyItem?.Invoke(param);
            this.View.OnSelect  = () => param.OnSelectItem?.Invoke(param);
            this.SetButtonStatus(param);
        }

        private void SetButtonStatus(ItemCollectionItemModel param)
        {
            var isCoin     = param.UITemplateItemRecord.UnlockType == UITemplateItemData.UnlockType.SoftCurrency;
            var isAds      = param.UITemplateItemRecord.UnlockType == UITemplateItemData.UnlockType.Ads;
            var isIap      = param.UITemplateItemRecord.UnlockType == UITemplateItemData.UnlockType.IAP;
            var isOwner    = param.ItemData.CurrentStatus == UITemplateItemData.Status.Owned;
            var isUnlocked = param.ItemData.CurrentStatus == UITemplateItemData.Status.Unlocked;
            var isLocked   = param.ItemData.CurrentStatus == UITemplateItemData.Status.Locked;

            this.View.btnBuyCoin.gameObject.SetActive(isCoin && !isOwner && isUnlocked);
            this.View.btnSelect.gameObject.SetActive(!isLocked);
            this.View.btnBuyAds.gameObject.SetActive(isAds && !isOwner && isUnlocked);
            this.View.btnBuyIap.gameObject.SetActive(isIap && !isOwner && isUnlocked);
        }
    }
}