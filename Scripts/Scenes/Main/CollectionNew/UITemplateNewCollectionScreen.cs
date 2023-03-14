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
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TheOneStudio.UITemplate.UITemplate.Interfaces;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using Zenject;

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

        private readonly List<ItemCollectionItemModel> itemCollectionItemModels = new();

        private readonly List<TopButtonItemModel> topButtonItemModels = new();

        private int         currentSelectedCategoryIndex;
        private IDisposable randomTimerDispose;

        public UITemplateNewCollectionScreenPresenter(SignalBus               signalBus,               EventSystem                       eventSystem,   IIapServices iapServices, ILogService                     logger, UITemplateAdServiceWrapper uiTemplateAdServiceWrapper,
                                                      IGameAssets             gameAssets,              ScreenManager                     screenManager, DiContainer  diContainer, UITemplateCategoryItemBlueprint uiTemplateCategoryItemBlueprint,
                                                      UITemplateItemBlueprint uiTemplateItemBlueprint, UITemplateInventoryDataController uiTemplateInventoryDataController,
                                                      UITemplateInventoryData uiTemplateInventoryData, UITemplateSoundServices           soundServices) : base(signalBus)
        {
            this.eventSystem                       = eventSystem;
            this.iapServices                       = iapServices;
            this.logger                            = logger;
            this.uiTemplateAdServiceWrapper        = uiTemplateAdServiceWrapper;
            this.gameAssets                        = gameAssets;
            this.ScreenManager                     = screenManager;
            this.diContainer                       = diContainer;
            this.uiTemplateCategoryItemBlueprint   = uiTemplateCategoryItemBlueprint;
            this.uiTemplateItemBlueprint           = uiTemplateItemBlueprint;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
            this.uiTemplateInventoryData           = uiTemplateInventoryData;
            this.SoundServices                     = soundServices;
        }

        protected virtual int CoinAddAmount => 500;

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.btnHome.onClick.AddListener(this.OnClickHomeButton);
            this.View.btnUnlockRandom.onClick.AddListener(this.OnClickUnlockRandomButton);
            this.View.btnAddMoreCoin.onClick.AddListener(this.OnClickAddMoreCoinButton);
        }

        public override async void BindData()
        {
            this.View.coinText.Subscribe(this.SignalBus, this.uiTemplateInventoryDataController.GetCurrency().Value);

            this.itemCollectionItemModels.Clear();
            this.PrePareModel();
            await this.BindDataCollectionForAdapter();
            this.OnButtonCategorySelected(this.topButtonItemModels[0]);
        }

        protected virtual void OnClickAddMoreCoinButton()
        {
            this.uiTemplateAdServiceWrapper.ShowRewardedAd(placement, () =>
            {
                var currencyData = this.uiTemplateInventoryDataController.GetCurrency();
                currencyData.Value += this.CoinAddAmount;
                this.uiTemplateInventoryDataController.UpdateCurrency(currencyData.Value);
            });
        }

        protected virtual void OnClickUnlockRandomButton()
        {
            this.uiTemplateAdServiceWrapper.ShowRewardedAd(placement, () =>
            {
                this.eventSystem.enabled = false;
                var currentCategory = this.uiTemplateCategoryItemBlueprint.ElementAt(this.currentSelectedCategoryIndex).Value.Id;

                var collectionModel = this.itemCollectionItemModels
                                          .Where(x => x.UITemplateItemRecord.Category.Equals(currentCategory) &&
                                                      !this.uiTemplateInventoryData.IDToItemData.ContainsKey(x.ItemData.Id)).ToList();

                foreach (var model in this.itemCollectionItemModels) model.IndexItemSelected = -1;

                var maxTime = collectionModel.Count == 1 ? 0.3f : 3;

                collectionModel.GachaItemWithTimer(this.randomTimerDispose, model =>
                {
                    foreach (var itemCollectionItemModel in collectionModel) itemCollectionItemModel.IndexItemSelected = model.ItemIndex;

                    this.OnRandomItemComplete(model);
                    this.BuyItemCompleted(model);
                    this.eventSystem.enabled = true;
                }, model =>
                {
                    foreach (var itemCollectionItemModel in collectionModel) itemCollectionItemModel.IndexItemSelected = model.ItemIndex;

                    this.View.itemCollectionGridAdapter.Refresh();
                }, maxTime, 0.1f);
            });
        }

        protected virtual void OnRandomItemComplete(ItemCollectionItemModel model)
        {
        }

        protected virtual async void OnClickHomeButton()
        {
            await this.ScreenManager.OpenScreen<UITemplateHomeTapToPlayScreenPresenter>();
        }

        private void PrePareModel()
        {
            this.itemCollectionItemModels.Clear();

            foreach (var record in this.uiTemplateItemBlueprint.Values)
            {
                var itemData = this.uiTemplateInventoryDataController.HasItem(record.Id)
                    ? this.uiTemplateInventoryDataController.GetItemData(record.Id)
                    : this.uiTemplateInventoryDataController.GetItemData(record.Id, UITemplateItemData.Status.Unlocked);

                var model = new ItemCollectionItemModel
                {
                    UITemplateItemRecord = record, OnBuyItem = this.OnBuyItem, OnSelectItem = this.OnSelectItem, ItemData = itemData
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
                var collectionModel = this.itemCollectionItemModels.Where(x => x.UITemplateItemRecord.Category.Equals(currentCategory)).ToList();

                var itemSelected = this.uiTemplateInventoryDataController.GetCurrentItemSelected(currentCategory);

                var indexSelected = collectionModel.FindIndex(x => x.ItemData.Id.Equals(itemSelected));

                for (var j = 0; j < collectionModel.Count; j++)
                {
                    var currentModel = collectionModel[j];
                    currentModel.ItemIndex         = j;
                    currentModel.IndexItemSelected = indexSelected == -1 ? 0 : indexSelected;
                }
            }

            await this.View.topButtonBarAdapter.InitItemAdapter(this.topButtonItemModels, this.diContainer);
        }

        private async void OnButtonCategorySelected(TopButtonItemModel obj)
        {
            //refresh top button bar
            this.currentSelectedCategoryIndex = obj.Index;

            foreach (var topButtonItemModel in this.topButtonItemModels) topButtonItemModel.SelectedIndex = this.currentSelectedCategoryIndex;

            //Bind Data Collection
            var currentCategory = this.uiTemplateCategoryItemBlueprint.ElementAt(this.currentSelectedCategoryIndex).Value.Id;
            var tempModel       = this.itemCollectionItemModels.Where(x => x.UITemplateItemRecord.Category.Equals(currentCategory)).ToList();

            await this.View.itemCollectionGridAdapter.InitItemAdapter(tempModel, this.diContainer);
            this.View.topButtonBarAdapter.Refresh();
            var hasOwnAllItem = tempModel.All(x => this.uiTemplateInventoryDataController.HasItem(x.ItemData.Id));
            this.View.btnUnlockRandom.gameObject.SetActive(!hasOwnAllItem);
        }

        private void OnSelectItem(ItemCollectionItemModel obj)
        {
            // If the item is not owned, do not select it
            if (!this.uiTemplateInventoryDataController.TryGetItemData(obj.ItemData.Id, out var itemData) || itemData.CurrentStatus != UITemplateItemData.Status.Owned) return;

            var currentCategory = this.uiTemplateCategoryItemBlueprint.ElementAt(this.currentSelectedCategoryIndex).Value.Id;
            var tempModel       = this.itemCollectionItemModels.Where(x => x.UITemplateItemRecord.Category.Equals(currentCategory)).ToList();

            foreach (var model in tempModel) model.IndexItemSelected = obj.ItemIndex;

            // Save the selected item
            this.uiTemplateInventoryDataController.UpdateCurrentSelectedItem(currentCategory, obj.ItemData.Id);
            this.OnSelectedItem(obj.ItemData);

            this.View.itemCollectionGridAdapter.Refresh();
        }

        protected virtual void OnSelectedItem(UITemplateItemData itemData)
        {
        }

        private void OnBuyItem(ItemCollectionItemModel obj)
        {
            switch (obj.UITemplateItemRecord.UnlockType)
            {
                case UITemplateItemData.UnlockType.Ads:
                    this.BuyWithAds(obj);

                    break;
                case UITemplateItemData.UnlockType.SoftCurrency:
                    this.BuyWithSoftCurrency(obj);

                    break;
                case UITemplateItemData.UnlockType.None:
                    break;
                case UITemplateItemData.UnlockType.IAP:
                    this.BuyWithIAP(obj);

                    break;
                case UITemplateItemData.UnlockType.Progression:
                    break;
                case UITemplateItemData.UnlockType.Gift:
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

        private readonly   DiContainer                       diContainer;
        private readonly   UITemplateCategoryItemBlueprint   uiTemplateCategoryItemBlueprint;
        private readonly   UITemplateItemBlueprint           uiTemplateItemBlueprint;
        private readonly   UITemplateInventoryDataController uiTemplateInventoryDataController;
        private readonly   UITemplateInventoryData           uiTemplateInventoryData;
        private readonly   EventSystem                       eventSystem;
        private readonly   IIapServices                      iapServices;
        private readonly   ILogService                       logger;
        private readonly   UITemplateAdServiceWrapper        uiTemplateAdServiceWrapper;
        private readonly   IGameAssets                       gameAssets;
        protected readonly UITemplateSoundServices           SoundServices;
        protected readonly IScreenManager                    ScreenManager;

        #endregion

        #region Buy Item

        private void BuyWithSoftCurrency(ItemCollectionItemModel obj)
        {
            var currentCoin = this.uiTemplateInventoryDataController.GetCurrency(obj.UITemplateItemRecord.CurrencyID);

            if (currentCoin.Value < obj.UITemplateItemRecord.Price)
            {
                this.logger.Log($"Not Enough {obj.UITemplateItemRecord.CurrencyID}\nCurrent: {currentCoin.Value}, Needed: {obj.UITemplateItemRecord.Price}");

                return;
            }

            currentCoin.Value -= obj.UITemplateItemRecord.Price;
            this.uiTemplateInventoryDataController.UpdateCurrency(currentCoin.Value, obj.UITemplateItemRecord.CurrencyID);
            this.BuyItemCompleted(obj);
        }

        private void BuyWithAds(ItemCollectionItemModel obj)
        {
            this.uiTemplateAdServiceWrapper.ShowRewardedAd(placement, () =>
            {
                this.BuyItemCompleted(obj);
            });
        }

        private void BuyWithIAP(ItemCollectionItemModel obj)
        {
            this.iapServices.BuyProductID(obj.UITemplateItemRecord.CurrencyID, x =>
            {
                this.BuyItemCompleted(obj);
            });
        }

        private void BuyItemCompleted(ItemCollectionItemModel obj)
        {
            obj.ItemData.CurrentStatus = UITemplateItemData.Status.Owned;
            this.uiTemplateInventoryDataController.AddItemData(obj.ItemData);
            this.uiTemplateInventoryData.CategoryToChosenItem[obj.UITemplateItemRecord.Category] = obj.UITemplateItemRecord.Id;
            this.uiTemplateInventoryDataController.UpdateCurrentSelectedItem(obj.UITemplateItemRecord.Category, obj.UITemplateItemRecord.Id);
            this.OnSelectItem(obj);
        }

        #endregion
    }
}