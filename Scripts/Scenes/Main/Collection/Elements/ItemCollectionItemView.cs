namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.Collection.Elements
{
    using System;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.AssetLibrary;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.Collection.Base;
    using UnityEngine;
    using UnityEngine.Scripting;

    public class ItemCollectionItemModel : BaseItemCollectionModel
    {
        public Action<ItemCollectionItemModel> OnSelected { get; set; }
        public Action<ItemCollectionItemModel> OnBuy      { get; set; }
    }

    public class ItemCollectionItemView : BaseItemCollectionView
    {
    }

    public class ItemCollectionItemPresenter : BaseItemCollectionPresenter<ItemCollectionItemView, ItemCollectionItemModel>
    {
        private readonly IGameAssets             gameAssets;
        private readonly UITemplateInventoryData inventoryData;

        private ItemCollectionItemModel model;

        [Preserve]
        public ItemCollectionItemPresenter(IGameAssets gameAssets, UITemplateInventoryData inventoryData) : base(gameAssets)
        {
            this.gameAssets    = gameAssets;
            this.inventoryData = inventoryData;
        }

        protected override string CategoryType => "Item";

        public override async void BindData(ItemCollectionItemModel param)
        {
            this.model = param;
            this.Init(param.UITemplateItemInventoryData.CurrentStatus);
            this.inventoryData.CategoryToChosenItem.Values.Any(value => value.Equals(this.model.UITemplateItemInventoryData.Id));
            this.View.ItemImage.sprite = await this.gameAssets.LoadAssetAsync<Sprite>(this.model.UITemplateItemInventoryData.ItemBlueprintRecord.ImageAddress);
            this.View.PriceText.text   = $"{param.UITemplateItemInventoryData.ShopBlueprintRecord.Price}";
            this.View.SelectButton.onClick.AddListener(this.OnSelect);
            this.View.BuyItemButton.onClick.AddListener(this.OnBuyItem);
        }

        private void OnSelect()
        {
            this.model.OnSelected?.Invoke(this.model);
        }

        private void OnBuyItem()
        {
            // if have enough money
            this.model.OnBuy?.Invoke(this.model);
            // if not enough money
            // this.model.OnNotEnoughMoney?.Invoke();
            // endif
        }

        private void Init(UITemplateItemData.Status status)
        {
            this.Init();
            switch (status)
            {
                case UITemplateItemData.Status.Owned:
                    this.InitItemOwned();

                    break;
                case UITemplateItemData.Status.Unlocked:
                    this.InitItemUnLocked();

                    break;
                case UITemplateItemData.Status.Locked:
                    this.InitItemLocked();

                    break;
                case UITemplateItemData.Status.InProgress:
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

        private void InitItemLocked()
        {
            this.View.LockedObj.SetActive(true);
        }

        private void InitItemUnLocked()
        {
            this.View.UnlockedObj.SetActive(true);
        }

        private void InitItemOwned()
        {
            this.View.OwnedObj.SetActive(true);
        }

        public override void Dispose()
        {
            base.Dispose();
            this.View.SelectButton.onClick.RemoveAllListeners();
            this.View.BuyItemButton.onClick.RemoveAllListeners();
        }
    }
}