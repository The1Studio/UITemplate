namespace UITemplate.Scripts.Scenes.Main.Collection
{
    using System;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using TMPro;
    using UITemplate.Scripts.Models;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITemplateCollectionItemView : TViewMono
    {
        public GameObject      UnlockedObj;
        public GameObject      LockedObj;
        public GameObject      OwnedObj;
        public GameObject      ChoosedObj;
        public Image           ItemImage;
        public TextMeshProUGUI PriceText;
        public Button          SelectButton;
        public Button          BuyItemButton;

        public void Init(ItemData.Status status)
        {
            this.Init();
            switch (status)
            {
                case ItemData.Status.Owned: this.InitItemOwned();
                    break;
                case ItemData.Status.Unlocked: this.InitItemUnLocked();
                    break;
                case ItemData.Status.Locked: this.InitItemLocked();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }

        private void Init()
        {
            this.LockedObj.SetActive(false);
            this.OwnedObj.SetActive(false);
            this.UnlockedObj.SetActive(false);
        }

        private void InitItemLocked()
        {
            this.LockedObj.SetActive(true);
        }

        private void InitItemUnLocked()
        {
            this.UnlockedObj.SetActive(true);
        }

        private void InitItemOwned()
        {
            this.OwnedObj.SetActive(true);
        }
    }

    public class UITemplateCollectionItemModel
    {
        public ItemData ItemData;
        public string   Category;

        public UITemplateCollectionItemModel(ItemData itemData, string category)
        {
            this.ItemData = itemData;
            this.Category = category;
        }
    }

    public class UITemplateCollectionItemPresenter : BaseUIItemPresenter<UITemplateCollectionItemView, UITemplateCollectionItemModel>
    {
        private readonly IGameAssets        gameAssets;
        private readonly UITemplateUserData userData;

        private UITemplateCollectionItemModel model;

        public UITemplateCollectionItemPresenter(IGameAssets gameAssets, UITemplateUserData userData) : base(gameAssets)
        {
            this.gameAssets = gameAssets;
            this.userData   = userData;
        }

        public override async void BindData(UITemplateCollectionItemModel param)
        {
            this.View.Init(param.ItemData.CurrentStatus);
            this.View.ItemImage.sprite = await this.gameAssets.LoadAssetAsync<Sprite>(this.model.ItemData.BlueprintRecord.Name);
            this.InitButton();
        }

        private void InitButton()
        {
            this.View.SelectButton.onClick.AddListener(this.OnSelect);
            this.View.BuyItemButton.onClick.AddListener(this.OnBuyItem);
        }

        private void OnSelect() {  }

        private void OnBuyItem() {  }

        private void OnWatchAds() {  }

        public override void Dispose()
        {
            base.Dispose();
            this.View.SelectButton.onClick.RemoveAllListeners();
            this.View.BuyItemButton.onClick.RemoveAllListeners();
        }
    }
}