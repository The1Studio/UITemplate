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
    using GameFoundation.Signals;
    using ServiceImplementation.IAPServices;
    using TheOne.Logging;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.IapScene;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.Scripting;
    using UnityEngine.UI;

    public class UITemplateNewCollectionScreen : BaseView
    {
        public Button                    btnHome;
        public Button                    btnUnlockRandom;
        public Button                    btnAddMoreCoin;
        public TopButtonBarAdapter       topButtonBarAdapter;
        public ItemCollectionGridAdapter itemCollectionGridAdapter;
        public UITemplateCurrencyView    coinText;
    }

    [ScreenInfo(nameof(UITemplateNewCollectionScreen))]
    public class UITemplateNewCollectionScreenPresenter : UITemplateBaseScreenPresenter<UITemplateNewCollectionScreen>
    {
        private static readonly string placement = "Collection";

        protected readonly List<ItemCollectionItemModel> itemCollectionItemModels = new();

        protected readonly List<TopButtonItemModel> topButtonItemModels = new();

        private int         currentSelectedCategoryIndex;
        private IDisposable randomTimerDispose;

        [Preserve]
        public UITemplateNewCollectionScreenPresenter(
            SignalBus                         signalBus,
            ILoggerManager                    logger,
            EventSystem                       eventSystem,
            IIapServices                      iapServices,
            UITemplateAdServiceWrapper        uiTemplateAdServiceWrapper,
            IGameAssets                       gameAssets,
            IScreenManager                    screenManager,
            UITemplateCategoryItemBlueprint   uiTemplateCategoryItemBlueprint,
            UITemplateItemBlueprint           uiTemplateItemBlueprint,
            UITemplateInventoryDataController uiTemplateInventoryDataController,
            UITemplateLevelDataController     levelDataController
        ) : base(signalBus, logger)
        {
            this.eventSystem                       = eventSystem;
            this.iapServices                       = iapServices;
            this.uiTemplateAdServiceWrapper        = uiTemplateAdServiceWrapper;
            this.gameAssets                        = gameAssets;
            this.ScreenManager                     = screenManager;
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
            this.View.btnUnlockRandom.onClick.AddListener(this.OnClickUnlockRandomButton);
            this.View.btnAddMoreCoin.onClick.AddListener(this.OnClickAddMoreCoinButton);
        }

        public override async UniTask BindData()
        {
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

        private void BuyItemCompleted()
        {
            this.uiTemplateInventoryDataController.AddCurrency(this.CoinAddAmount, UITemplateInventoryDataController.DefaultSoftCurrencyID, placement,
                startAnimationRect: this.View.btnAddMoreCoin.transform as RectTransform);
            this.View.itemCollectionGridAdapter.Refresh();
        }

        protected virtual void OnClickUnlockRandomButton()
        {
            this.uiTemplateAdServiceWrapper.ShowRewardedAd(placement,
                () =>
                {
                    this.eventSystem.enabled = false;
                    var currentCategory = this.uiTemplateCategoryItemBlueprint.ElementAt(this.currentSelectedCategoryIndex).Value.Id;

                    var collectionModel = this.itemCollectionItemModels
                        .Where(x => x.ItemBlueprintRecord.Category.Equals(currentCategory) && !this.uiTemplateInventoryDataController.HasItem(x.ItemData.Id)).ToList();

                    foreach (var model in this.itemCollectionItemModels) model.IndexItemSelected = -1;

                    var maxTime = collectionModel.Count == 1 ? 0.3f : 3;

                    collectionModel.GachaItemWithTimer(this.randomTimerDispose,
                        model =>
                        {
                            foreach (var itemCollectionItemModel in collectionModel) itemCollectionItemModel.IndexItemSelected = model.ItemIndex;

                            this.OnRandomItemComplete(model);
                            this.BuyItemCompleted(model);
                            this.eventSystem.enabled = true;
                        },
                        model =>
                        {
                            foreach (var itemCollectionItemModel in collectionModel) itemCollectionItemModel.IndexItemSelected = model.ItemIndex;

                            this.View.itemCollectionGridAdapter.Refresh();
                        },
                        maxTime,
                        0.1f);
                });
        }

        protected virtual void OnRandomItemComplete(ItemCollectionItemModel model) { }

        protected virtual async void OnClickHomeButton() { await this.ScreenManager.OpenScreen<UITemplateHomeTapToPlayScreenPresenter>(); }

        private void PrePareModel()
        {
            this.itemCollectionItemModels.Clear();

            var unlockType = this.levelDataController.UnlockedFeature;

            foreach (var record in this.uiTemplateItemBlueprint.Values)
            {
                var itemData = this.uiTemplateInventoryDataController.GetItemData(record.Id, Status.Unlocked);

                if ((itemData.ShopBlueprintRecord.UnlockType & unlockType) == 0) continue;

                var model = new ItemCollectionItemModel
                {
                    OnBuyItem = this.OnBuyItem, OnSelectItem = this.OnSelectItem, OnUseItem = this.OnUseItem, ItemData = itemData,
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

                this.topButtonItemModels.Add(new() { Title = record.Title, Icon = icon, OnSelected = this.OnButtonCategorySelected, SelectedIndex = this.currentSelectedCategoryIndex, Index = index });

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

            this.RebindModelData();
            await this.View.topButtonBarAdapter.InitItemAdapter(this.topButtonItemModels);
        }

        protected virtual void RebindModelData() { }

        protected virtual async void OnButtonCategorySelected(TopButtonItemModel obj)
        {
            //refresh top button bar
            this.currentSelectedCategoryIndex = obj.Index;

            foreach (var topButtonItemModel in this.topButtonItemModels) topButtonItemModel.SelectedIndex = this.currentSelectedCategoryIndex;

            //Bind Data Collection
            var currentCategory = this.uiTemplateCategoryItemBlueprint.ElementAt(this.currentSelectedCategoryIndex).Value.Id;
            var tempModel       = this.itemCollectionItemModels.Where(x => x.ItemBlueprintRecord.Category.Equals(currentCategory)).ToList();

            await this.View.itemCollectionGridAdapter.InitItemAdapter(tempModel);
            this.View.topButtonBarAdapter.Refresh();
            var hasOwnAllItem = tempModel.All(x => this.uiTemplateInventoryDataController.HasItem(x.ItemData.Id));
            this.View.btnUnlockRandom.gameObject.SetActive(!hasOwnAllItem);
        }

        private void OnUseItem(ItemCollectionItemModel obj)
        {
            // If the item is not owned, do not use it
            if (!this.uiTemplateInventoryDataController.TryGetItemData(obj.ItemData.Id, out var itemData) || itemData.CurrentStatus != Status.Owned) return;

            var currentCategory = this.uiTemplateCategoryItemBlueprint.ElementAt(this.currentSelectedCategoryIndex).Value.Id;
            var tempModel       = this.itemCollectionItemModels.Where(x => x.ItemBlueprintRecord.Category.Equals(currentCategory)).ToList();

            foreach (var model in tempModel) model.IndexItemUsed = model.IndexItemSelected = obj.ItemIndex;

            // Save the selected item
            this.uiTemplateInventoryDataController.UpdateCurrentSelectedItem(currentCategory, obj.ItemData.Id);
            this.OnUsedItem(obj.ItemData);

            this.View.itemCollectionGridAdapter.Refresh();
        }

        protected virtual void OnUsedItem(UITemplateItemData itemData) { }

        private void OnSelectItem(ItemCollectionItemModel obj)
        {
            var currentCategory = this.uiTemplateCategoryItemBlueprint.ElementAt(this.currentSelectedCategoryIndex).Value.Id;
            var tempModel       = this.itemCollectionItemModels.Where(x => x.ItemBlueprintRecord.Category.Equals(currentCategory)).ToList();

            foreach (var model in tempModel) model.IndexItemSelected = obj.ItemIndex;

            // Save the selected item
            this.OnSelectedItem(obj.ItemData);

            this.View.itemCollectionGridAdapter.Refresh();
        }

        protected virtual void OnSelectedItem(UITemplateItemData itemData) { }

        private void OnBuyItem(ItemCollectionItemModel obj)
        {
            switch (obj.ShopBlueprintRecord.UnlockType)
            {
                case UnlockType.Ads:
                    this.BuyWithAds(obj);

                    break;
                case UnlockType.SoftCurrency:
                    this.BuyWithSoftCurrency(obj);

                    break;
                case UnlockType.None: break;
                case UnlockType.IAP:
                    this.BuyWithIAP(obj);

                    break;
                case UnlockType.StartedPack:
                    this.BuyWithStartedPack(obj);

                    break;

                case UnlockType.Progression: break;
                case UnlockType.Gift:        break;
                case UnlockType.DailyReward:
                    this.BuyWithDailyReward(obj);

                    break;
                case UnlockType.LuckySpin:
                    this.BuyWithLuckySpin(obj);

                    break;
                case UnlockType.All: break;
                default:             throw new ArgumentOutOfRangeException();
            }
        }

        private void BuyWithStartedPack(ItemCollectionItemModel itemCollectionItemModel)
        {
            this.ScreenManager.OpenScreen<UITemplateStartPackScreenPresenter, UITemplateStaterPackModel>(new()
            {
                OnComplete = this.OnBuyStartedPackComplete,
            });
        }

        protected virtual void OnBuyStartedPackComplete(string packId, int quantity) { }

        public override void Dispose()
        {
            base.Dispose();
            this.randomTimerDispose?.Dispose();
        }

        #region inject

        private readonly   UITemplateCategoryItemBlueprint   uiTemplateCategoryItemBlueprint;
        private readonly   UITemplateItemBlueprint           uiTemplateItemBlueprint;
        private readonly   UITemplateInventoryDataController uiTemplateInventoryDataController;
        private readonly   EventSystem                       eventSystem;
        private readonly   IIapServices                      iapServices;
        private readonly   UITemplateAdServiceWrapper        uiTemplateAdServiceWrapper;
        private readonly   IGameAssets                       gameAssets;
        private readonly   UITemplateLevelDataController     levelDataController;
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

        protected virtual void BuyWithSoftCurrency(ItemCollectionItemModel obj, Action onFail = null)
        {
            var currentCoin = this.uiTemplateInventoryDataController.GetCurrencyValue(obj.ShopBlueprintRecord.CurrencyID);

            if (currentCoin < obj.ShopBlueprintRecord.Price)
            {
                this.Logger.Info($"Not Enough {obj.ShopBlueprintRecord.CurrencyID}\nCurrent: {currentCoin}, Needed: {obj.ShopBlueprintRecord.Price}");
                onFail?.Invoke();
                return;
            }

            this.uiTemplateInventoryDataController.AddCurrency(-obj.ShopBlueprintRecord.Price, obj.ShopBlueprintRecord.CurrencyID, placement);
            this.BuyItemCompleted(obj);
        }

        private void BuyWithAds(ItemCollectionItemModel obj)
        {
            this.uiTemplateAdServiceWrapper.ShowRewardedAd(placement,
                () => { this.BuyItemCompleted(obj); });
        }

        private void BuyWithIAP(ItemCollectionItemModel obj)
        {
            this.iapServices.BuyProductID(obj.ShopBlueprintRecord.Id,
                (x, quantity) => { this.BuyItemCompleted(obj); });
        }

        private void BuyItemCompleted(ItemCollectionItemModel obj)
        {
            obj.ItemData.RemainingAdsProgress--;

            if (obj.ItemData.RemainingAdsProgress > 0) return;
            this.uiTemplateInventoryDataController.SetOwnedItemData(obj.ItemData, true);
            this.OnUseItem(obj);
        }

        #endregion
    }
}