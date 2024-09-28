namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.CollectionNew
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.AssetLibrary;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine;
    using UnityEngine.Scripting;

    // Rebind this class to your own item view
    public class UITemplateCollectionItemViewHelper
    {
        protected readonly IGameAssets                       GameAssets;
        private readonly   UITemplateInventoryDataController uiTemplateInventoryDataController;

        [Preserve]
        public UITemplateCollectionItemViewHelper(IGameAssets gameAssets, UITemplateInventoryDataController uiTemplateInventoryDataController)
        {
            this.GameAssets                        = gameAssets;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
        }

        public virtual async void BindDataItem(ItemCollectionItemModel param, ItemCollectionItemView view)
        {
            view.imgIcon.sprite = await this.GameAssets.LoadAssetAsync<Sprite>(param.ItemBlueprintRecord.ImageAddress);
            view.txtPrice.text  = $"{param.ShopBlueprintRecord.Price}";

            this.SetViewItem(param, view);
        }

        public virtual void DisposeItem(ItemCollectionItemView view) { }

        protected virtual void SetViewItem(ItemCollectionItemModel param, ItemCollectionItemView view)
        {
            view.OnBuyAds         = () => param.OnBuyItem?.Invoke(param);
            view.OnBuyCoin        = () => param.OnBuyItem?.Invoke(param);
            view.OnBuyIap         = () => param.OnBuyItem?.Invoke(param);
            view.OnSelect         = () => param.OnSelectItem?.Invoke(param);
            view.OnUse            = () => param.OnUseItem?.Invoke(param);
            view.OnBuyDailyReward = () => param.OnBuyItem?.Invoke(param);
            view.OnBuyLuckySpin   = () => param.OnBuyItem?.Invoke(param);
            view.OnBuyStartPack   = () => param.OnBuyItem?.Invoke(param);
            this.SetButtonStatusAndBorderStatus(param, view);
        }

        protected virtual void SetButtonStatusAndBorderStatus(ItemCollectionItemModel param, ItemCollectionItemView view)
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

            view.btnBuyCoin.gameObject.SetActive(isCoin && !isOwner && isUnlocked);
            view.imgLockBuyCoin.gameObject.SetActive(!this.IsItemBuyCoinAble(param));
            view.btnUse.gameObject.SetActive(isOwner && !isUse);
            view.btnSelect.gameObject.SetActive(!isLocked);
            view.btnBuyAds.gameObject.SetActive(isAds && !isOwner && isUnlocked);
            view.btnBuyIap.gameObject.SetActive(isIap && !isOwner && isUnlocked);
            view.btnDailyReward.gameObject.SetActive(isDaily && !isOwner && isUnlocked);
            view.btnLuckySpin.gameObject.SetActive(isLuckySpin && !isOwner && isUnlocked);
            view.btnStartPack.gameObject.SetActive(isStartPack && !isOwner && isUnlocked);

            view.objUsed.SetActive(param.ItemIndex == param.IndexItemUsed);
            view.objChoose.SetActive((!isStartPack || isStartPack && isOwner) && param.ItemIndex == param.IndexItemSelected && param.ItemIndex != param.IndexItemUsed);
            view.objChooseStaredPack.SetActive(isStartPack && !isOwner && param.ItemIndex == param.IndexItemSelected && param.ItemIndex != param.IndexItemUsed);
            view.objNormal.SetActive(param.ItemIndex != param.IndexItemSelected && (!isStartPack || isStartPack && isOwner));
            view.objStaredPack.SetActive(isStartPack && !isUse && !isOwner);
        }

        protected virtual bool IsItemBuyCoinAble(ItemCollectionItemModel param)
        {
            if (string.IsNullOrEmpty(param.ShopBlueprintRecord.CurrencyID))
            {
                return true;
            }

            var currentCoin = this.uiTemplateInventoryDataController.GetCurrencyValue(param.ShopBlueprintRecord.CurrencyID);

            return currentCoin >= param.ShopBlueprintRecord.Price;
        }
    }
}