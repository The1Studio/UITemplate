namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.CollectionNew
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateNewCollectionScreen : BaseView
    {
        public Button                    btnHome;
        public Button                    btnAddMoreCoin;
        public TopButtonBarAdapter       topButtonBarAdapter;
        public ItemCollectionGridAdapter itemCollectionGridAdapter;
        public UITemplateCurrencyView    coinText;
    }

    [ScreenInfo(nameof(UITemplateNewCollectionScreen))]
    public class UITemplateNewCollectionScreenPresenter : UITemplateBaseScreenPresenter<UITemplateNewCollectionScreen>
    {
        private static readonly string placement = "Collection";

        private readonly List<ItemCollectionItemModel> itemCollectionItemModels = new();

        private readonly List<TopButtonItemModel> topButtonItemModels = new();

        private int         currentSelectedCategoryIndex;
        private IDisposable randomTimerDispose;

        public UITemplateNewCollectionScreenPresenter(
            SignalBus                         signalBus,
            ILogService                       logger,
            UITemplateAdServiceWrapper        uiTemplateAdServiceWrapper,
            IGameAssets                       gameAssets,
            ScreenManager                     screenManager,
            DiContainer                       diContainer,
            UITemplateCategoryItemBlueprint   uiTemplateCategoryItemBlueprint,
            UITemplateItemBlueprint           uiTemplateItemBlueprint,
            UITemplateInventoryDataController uiTemplateInventoryDataController,
            UITemplateLevelDataController     levelDataController
        ) : base(signalBus)
        {
            this.logger                            = logger;
            this.uiTemplateAdServiceWrapper        = uiTemplateAdServiceWrapper;
            this.gameAssets                        = gameAssets;
            this.ScreenManager                     = screenManager;
            this.diContainer                       = diContainer;
            this.uiTemplateCategoryItemBlueprint   = uiTemplateCategoryItemBlueprint;
            this.uiTemplateItemBlueprint           = uiTemplateItemBlueprint;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
            this.levelDataController               = levelDataController;
        }

        protected virtual int CoinAddAmount => 500;

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.btnHome.onClick.AddListener(this.OnClickHomeButton);
            this.View.btnAddMoreCoin.onClick.AddListener(this.OnClickAddMoreCoinButton);
        }

        public override async UniTask BindData()
        {
            this.View.coinText.Subscribe(this.SignalBus, this.uiTemplateInventoryDataController.GetCurrencyValue());

            this.itemCollectionItemModels.Clear();
            this.PrePareModel();
            await this.BindDataCollectionForAdapter();
            this.OnButtonCategorySelected(this.topButtonItemModels[0]);
        }

        protected virtual void OnClickAddMoreCoinButton()
        {
            this.uiTemplateAdServiceWrapper.ShowRewardedAd(placement,
                                                           this.BuyItemCompleted);
        }

        private async void BuyItemCompleted()
        {
            await this.uiTemplateInventoryDataController.AddCurrency(this.CoinAddAmount, startAnimationRect: this.View.btnAddMoreCoin.transform as RectTransform);
            this.View.itemCollectionGridAdapter.Refresh();
        }

        protected virtual async void OnClickHomeButton()
        {
            await this.ScreenManager.OpenScreen<UITemplateHomeTapToPlayScreenPresenter>();
        }

        private void PrePareModel()
        {
            this.itemCollectionItemModels.Clear();

            var unlockType = this.levelDataController.UnlockedFeature;

            foreach (var record in this.uiTemplateItemBlueprint.Values)
            {
                var itemData = this.uiTemplateInventoryDataController.GetItemData(record.Id, UITemplateItemData.Status.Unlocked);

                if ((itemData.ShopBlueprintRecord.UnlockType & unlockType) == 0) continue;

                var model = new ItemCollectionItemModel
                {
                    OnBuyItem = this.OnBuyItem, OnSelectItem = this.OnSelectItem, OnUseItem = this.OnUseItem, ItemData = itemData
                };

                this.itemCollectionItemModels.Add(model);
            }
        }

        private async UniTask BindDataCollectionForAdapter()
        {
            //TopBar
            this.currentSelectedCategoryIndex = 0;
            this.topButtonItemModels.Clear();

            var index = 0;

            foreach (var record in this.uiTemplateCategoryItemBlueprint.Values)
            {
                var icon = await this.gameAssets.LoadAssetAsync<Sprite>(record.Icon);

                this.topButtonItemModels.Add(new TopButtonItemModel { Icon = icon, OnSelected = this.OnButtonCategorySelected, SelectedIndex = this.currentSelectedCategoryIndex, Index = index });

                index++;
            }

            //Collection
            for (var i = 0; i < this.uiTemplateCategoryItemBlueprint.Count; i++)
            {
                var currentCategory = this.uiTemplateCategoryItemBlueprint.ElementAt(i).Value.Id;
                var collectionModel = this.itemCollectionItemModels.Where(x => x.ItemBlueprintRecord.Category.Equals(currentCategory)).ToList();

                var currentItemUsed = this.uiTemplateInventoryDataController.GetCurrentItemSelected(currentCategory);

                var indexUsed = collectionModel.FindIndex(x => x.ItemData.Id.Equals(currentItemUsed));

                for (var j = 0; j < collectionModel.Count; j++)
                {
                    var currentModel = collectionModel[j];
                    currentModel.ItemIndex     = j;
                    currentModel.IndexItemUsed = currentModel.IndexItemSelected = indexUsed == -1 ? 0 : indexUsed;
                }
            }

            await this.View.topButtonBarAdapter.InitItemAdapter(this.topButtonItemModels, this.diContainer);
        }

        protected virtual async void OnButtonCategorySelected(TopButtonItemModel obj)
        {
            //refresh top button bar
            this.currentSelectedCategoryIndex = obj.Index;

            foreach (var topButtonItemModel in this.topButtonItemModels) topButtonItemModel.SelectedIndex = this.currentSelectedCategoryIndex;

            //Bind Data Collection
            var currentCategory = this.uiTemplateCategoryItemBlueprint.ElementAt(this.currentSelectedCategoryIndex).Value.Id;
            var tempModel       = this.itemCollectionItemModels.Where(x => x.ItemBlueprintRecord.Category.Equals(currentCategory)).ToList();

            await this.View.itemCollectionGridAdapter.InitItemAdapter(tempModel, this.diContainer);
            this.View.topButtonBarAdapter.Refresh();
        }

        protected void OnUseItem(ItemCollectionItemModel obj)
        {
            // If the item is not owned, do not use it
            if (!this.uiTemplateInventoryDataController.TryGetItemData(obj.ItemData.Id, out var itemData) || itemData.CurrentStatus != UITemplateItemData.Status.Owned) return;

            var currentCategory = this.uiTemplateCategoryItemBlueprint.ElementAt(this.currentSelectedCategoryIndex).Value.Id;
            var tempModel       = this.itemCollectionItemModels.Where(x => x.ItemBlueprintRecord.Category.Equals(currentCategory)).ToList();

            foreach (var model in tempModel) model.IndexItemUsed = model.IndexItemSelected = obj.ItemIndex;

            // Save the selected item
            this.uiTemplateInventoryDataController.UpdateCurrentSelectedItem(currentCategory, obj.ItemData.Id);
            this.OnUsedItem(obj.ItemData);

            this.View.itemCollectionGridAdapter.Refresh();
        }

        protected virtual void OnUsedItem(UITemplateItemData itemData)
        {
        }

        private void OnSelectItem(ItemCollectionItemModel obj)
        {
            var currentCategory = this.uiTemplateCategoryItemBlueprint.ElementAt(this.currentSelectedCategoryIndex).Value.Id;
            var tempModel       = this.itemCollectionItemModels.Where(x => x.ItemBlueprintRecord.Category.Equals(currentCategory)).ToList();

            foreach (var model in tempModel) model.IndexItemSelected = obj.ItemIndex;

            // Save the selected item
            this.OnSelectedItem(obj.ItemData);

            this.View.itemCollectionGridAdapter.Refresh();
        }

        protected virtual void OnSelectedItem(UITemplateItemData itemData)
        {
        }

        private void OnBuyItem(ItemCollectionItemModel obj)
        {
            switch (obj.ShopBlueprintRecord.UnlockType)
            {
                case UITemplateItemData.UnlockType.Ads:
                    this.BuyWithAds(obj);

                    break;
                case UITemplateItemData.UnlockType.SoftCurrency:
                    this.BuyWithSoftCurrency(obj);

                    break;
                case UITemplateItemData.UnlockType.None:
                    break;
                case UITemplateItemData.UnlockType.Progression:
                    break;
                case UITemplateItemData.UnlockType.Gift:
                    break;
                case UITemplateItemData.UnlockType.DailyReward:
                    this.BuyWithDailyReward(obj);

                    break;
                case UITemplateItemData.UnlockType.LuckySpin:
                    this.BuyWithLuckySpin(obj);

                    break;
                case UITemplateItemData.UnlockType.All:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            this.randomTimerDispose?.Dispose();
            this.View.coinText.Unsubscribe(this.SignalBus);
        }

        #region inject

        protected readonly DiContainer                       diContainer;
        protected readonly UITemplateCategoryItemBlueprint   uiTemplateCategoryItemBlueprint;
        protected readonly UITemplateItemBlueprint           uiTemplateItemBlueprint;
        protected readonly UITemplateInventoryDataController uiTemplateInventoryDataController;
        protected readonly ILogService                       logger;
        protected readonly UITemplateAdServiceWrapper        uiTemplateAdServiceWrapper;
        protected readonly IGameAssets                       gameAssets;
        protected readonly UITemplateLevelDataController     levelDataController;
        protected readonly IScreenManager                    ScreenManager;

        #endregion

        #region Buy Item

        protected virtual void BuyWithDailyReward(ItemCollectionItemModel obj)
        {
            // _ = this.uiTemplateDailyRewardService.ShowDailyRewardPopupAsync(true);
        }

        protected virtual void BuyWithLuckySpin(ItemCollectionItemModel obj)
        {
            // this.uiTemplateLuckySpinServices.OpenLuckySpin();
        }

        private void BuyWithSoftCurrency(ItemCollectionItemModel obj)
        {
            var currentCoin = this.uiTemplateInventoryDataController.GetCurrencyValue(obj.ShopBlueprintRecord.CurrencyID);

            if (currentCoin < obj.ShopBlueprintRecord.Price)
            {
                this.logger.Log($"Not Enough {obj.ShopBlueprintRecord.CurrencyID}\nCurrent: {currentCoin}, Needed: {obj.ShopBlueprintRecord.Price}");

                return;
            }

            this.uiTemplateInventoryDataController.AddCurrency(-obj.ShopBlueprintRecord.Price, obj.ShopBlueprintRecord.CurrencyID);
            this.BuyItemCompleted(obj);
        }

        private void BuyWithAds(ItemCollectionItemModel obj)
        {
            this.uiTemplateAdServiceWrapper.ShowRewardedAd(placement, () =>
            {
                this.BuyItemCompleted(obj);
            });
        }

        protected virtual void BuyItemCompleted(ItemCollectionItemModel obj)
        {
            obj.ItemData.RemainingAdsProgress--;

            if (obj.ItemData.RemainingAdsProgress > 0)
            {
                return;
            }

            obj.ItemData.CurrentStatus = UITemplateItemData.Status.Owned;
            this.uiTemplateInventoryDataController.AddItemData(obj.ItemData);
            this.uiTemplateInventoryDataController.UpdateCurrentSelectedItem(obj.ItemBlueprintRecord.Category, obj.ItemBlueprintRecord.Id);
            this.OnSelectItem(obj);
        }

        #endregion
    }
}