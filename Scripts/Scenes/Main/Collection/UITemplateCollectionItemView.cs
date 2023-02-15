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
    using Zenject;

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
    }

    public class UITemplateCollectionItemModel
    {
        public int                                   Index            { get; set; }
        public ItemData                              ItemData         { get; set; }
        public string                                Category         { get; set; }
        public Action                                OnNotEnoughMoney { get; set; }
        public Action<UITemplateCollectionItemModel> OnSelected       { get; set; }
        public Action<UITemplateCollectionItemModel> OnBuy            { get; set; }
    }

    public class UITemplateCollectionItemPresenter : BaseUIItemPresenter<UITemplateCollectionItemView, UITemplateCollectionItemModel>
    {
        private readonly IGameAssets        gameAssets;
        private readonly UITemplateUserData userData;
        private readonly SignalBus          signalBus;

        private UITemplateCollectionItemModel model;

        public UITemplateCollectionItemPresenter(IGameAssets gameAssets, UITemplateUserData userData, SignalBus signalBus) : base(gameAssets)
        {
            this.gameAssets = gameAssets;
            this.userData   = userData;
            this.signalBus  = signalBus;
        }

        public override async void BindData(UITemplateCollectionItemModel param)
        {
            this.model = param;
            this.Init(param.ItemData.CurrentStatus);
            this.View.ChoosedObj.SetActive(this.model.ItemData.BlueprintRecord.Name.Equals(this.userData.UserPackageData.CurrentSelectCharacterId.Value));
            this.View.ItemImage.sprite = await this.gameAssets.LoadAssetAsync<Sprite>(this.model.ItemData.BlueprintRecord.Name);
            this.View.PriceText.text   = $"{param.ItemData.BlueprintRecord.Price}";
            this.View.SelectButton.onClick.AddListener(this.OnSelect);
            this.View.BuyItemButton.onClick.AddListener(this.OnBuyItem);
            this.View.BuyItemButton.gameObject.SetActive(param.ItemData.CurrentStatus is not ItemData.Status.Owned or ItemData.Status.Locked);
        }

        private void OnSelect()
        {
            this.userData.UserPackageData.CurrentSelectCharacterId.Value = this.model.ItemData.BlueprintRecord.Name;
            this.model.OnSelected?.Invoke(this.model);
        }

        private void OnBuyItem()
        {
            //handle not enought coin
            // this.model.OnNotEnoughMoney?.Invoke();
            // return;

            // if (this.model.Category.Equals())
            // {
            //     this.userData.ShopData.UpdateStatusItemData(this.model.ItemData.BlueprintRecord.Name, ItemData.Status.Owned);
            //     this.userData.UserPackageData.CurrentSelectCharacterId.Value = this.model.ItemData.BlueprintRecord.Name;
            // }
        }

        private void Init(ItemData.Status status)
        {
            this.Init();
            switch (status)
            {
                case ItemData.Status.Owned:
                    this.InitItemOwned();

                    break;
                case ItemData.Status.Unlocked:
                    this.InitItemUnLocked();

                    break;
                case ItemData.Status.Locked:
                    this.InitItemLocked();

                    break;
                case ItemData.Status.InProgress:
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }

        private void Init()
        {
            this.View.LockedObj.SetActive(false);
            this.View.OwnedObj.SetActive(false);
            this.View.UnlockedObj.SetActive(false);
        }

        private void InitItemLocked() { this.View.LockedObj.SetActive(true); }

        private void InitItemUnLocked() { this.View.UnlockedObj.SetActive(true); }

        private void InitItemOwned() { this.View.OwnedObj.SetActive(true); }

        public override void Dispose()
        {
            base.Dispose();
            this.View.SelectButton.onClick.RemoveAllListeners();
            this.View.BuyItemButton.onClick.RemoveAllListeners();
        }
    }
}