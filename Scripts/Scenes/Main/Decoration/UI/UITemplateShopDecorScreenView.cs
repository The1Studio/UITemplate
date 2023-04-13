namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.Decoration.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using ServiceImplementation.IAPServices;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.CollectionNew;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;
    using Object = UnityEngine.Object;

    public class UITemplateShopDecorScreenView : BaseView
    {
        public Button                          backButton;
        public UITemplateItemDecorationAdapter decorationItemAdapter;
        public UITemplateCurrencyView          coinText;
        public DecorCategoryTabView            categoryTabViewPrefab;
        public Transform                       categoryTabHolder;
    }

    [ScreenInfo(nameof(UITemplateShopDecorScreenView))]
    public class UITemplateDecorScreenPresenter : UITemplateBaseScreenPresenter<UITemplateShopDecorScreenView>
    {
        #region Inject

        protected readonly UITemplateDecorationManager       uiTemplateDecorationManager;
        protected readonly UITemplateInventoryDataController uiTemplateInventoryDataController;
        protected readonly IScreenManager                    screenManager;
        protected readonly UITemplateItemBlueprint           uiTemplateItemBlueprint;
        protected readonly IUnityIapServices                 unityUnityIapServices;
        protected readonly UITemplateInventoryData           uiTemplateInventoryData;
        protected readonly UITemplateAdServiceWrapper        uiTemplateAdServiceWrapper;
        protected readonly DiContainer                       diContainer;
        protected readonly UITemplateDecorCategoryBlueprint  uiTemplateDecorCategoryBlueprint;
        protected readonly UITemplateLuckySpinServices       uiTemplateLuckySpinServices;
        protected readonly UITemplateDailyRewardService      uiTemplateDailyRewardService;

        #endregion

        #region Cache

        private Dictionary<string, DecorCategoryTabView> idToCategoryTab          = new();
        private string                                   currentCategoryTab       = string.Empty;
        private List<ItemCollectionItemModel>            itemCollectionItemModels = new();

        #endregion

        protected virtual string           AdPlacement      => "Decoration";
        protected virtual string           DefaultTabActive => "Character";

        public UITemplateDecorScreenPresenter(SignalBus                         signalBus,
                                              UITemplateDecorationManager       uiTemplateDecorationManager,
                                              UITemplateInventoryDataController uiTemplateInventoryDataController,
                                              IScreenManager                    screenManager,
                                              UITemplateItemBlueprint           uiTemplateItemBlueprint,
                                              IUnityIapServices                 unityUnityIapServices,
                                              UITemplateInventoryData           uiTemplateInventoryData,
                                              UITemplateAdServiceWrapper        uiTemplateAdServiceWrapper,
                                              DiContainer                       diContainer,
                                              UITemplateDecorCategoryBlueprint  uiTemplateDecorCategoryBlueprint,
                                              UITemplateLuckySpinServices       uiTemplateLuckySpinServices,
                                              UITemplateDailyRewardService      uiTemplateDailyRewardService) : base(signalBus)
        {
            this.uiTemplateDecorationManager       = uiTemplateDecorationManager;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
            this.screenManager                     = screenManager;
            this.uiTemplateItemBlueprint           = uiTemplateItemBlueprint;
            this.unityUnityIapServices             = unityUnityIapServices;
            this.uiTemplateInventoryData           = uiTemplateInventoryData;
            this.uiTemplateAdServiceWrapper        = uiTemplateAdServiceWrapper;
            this.diContainer                       = diContainer;
            this.uiTemplateDecorCategoryBlueprint  = uiTemplateDecorCategoryBlueprint;
            this.uiTemplateLuckySpinServices       = uiTemplateLuckySpinServices;
            this.uiTemplateDailyRewardService      = uiTemplateDailyRewardService;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.itemCollectionItemModels.Clear();
            this.PrepareModel();
            this.View.backButton.onClick.AddListener(this.OnClickBackButton);
            this.InitCategoryTabs();
        }

        public override async UniTask BindData()
        {
            this.uiTemplateAdServiceWrapper.HideBannerAd();
            await UniTask.CompletedTask;
            this.View.coinText.Subscribe(this.SignalBus, this.uiTemplateInventoryDataController.GetCurrencyValue());
            this.uiTemplateDecorationManager.ShowDecorItems();
            await this.OnActiveTab(DefaultTabActive, true);
        }

        public override void Dispose()
        {
            base.Dispose();
            this.View.coinText.Unsubscribe(this.SignalBus);
            this.uiTemplateAdServiceWrapper.ShowBannerAd();
        }

        protected virtual async void OnClickBackButton()
        {
            this.CloseView();
        }

        private void InitCategoryTabs()
        {
            foreach (var (category, record) in this.uiTemplateDecorCategoryBlueprint)
            {
                var categoryTabView = Object.Instantiate(this.View.categoryTabViewPrefab, this.View.categoryTabHolder);
                this.idToCategoryTab.Add(record.Id, categoryTabView);
                categoryTabView.OnButtonClick += () => this.OnClickCategoryTab(category);
                categoryTabView.SetPosition(this.uiTemplateDecorationManager.GetDecoration(category).PositionUI);
            }
        }

        private async void OnClickCategoryTab(string category)
        {
            await this.OnActiveTab(category);
            this.uiTemplateDecorationManager.OnChangeCategory(this.currentCategoryTab);
        }

        private async UniTask BindDataToAdapter()
        {
            var listModels      = this.itemCollectionItemModels.Where(model => model.ItemBlueprintRecord.Category.Equals(this.currentCategoryTab)).ToList();
            var currentItemUsed = this.uiTemplateInventoryDataController.GetCurrentItemSelected(this.currentCategoryTab);
            var indexUsed       = listModels.FindIndex(item => item.ItemData.Id.Equals(currentItemUsed));
            for (var i = 0; i < listModels.Count; i++)
            {
                var currentModel = listModels[i];
                currentModel.ItemIndex     = i;
                currentModel.IndexItemUsed = currentModel.IndexItemSelected = indexUsed == -1 ? 0 : indexUsed;
            }

            await this.View.decorationItemAdapter.InitItemAdapter(listModels, this.diContainer);
        }

        private void PrepareModel()
        {
            this.itemCollectionItemModels.Clear();

            foreach (var record in this.uiTemplateItemBlueprint.Values)
            {
                var itemData = this.uiTemplateInventoryDataController.HasItem(record.Id)
                    ? this.uiTemplateInventoryDataController.GetItemData(record.Id)
                    : this.uiTemplateInventoryDataController.GetItemData(record.Id, UITemplateItemData.Status.Unlocked);

                var model = new ItemCollectionItemModel
                {
                    OnBuyItem = this.OnBuyItem, OnSelectItem = this.OnSelectItem, OnUseItem = this.OnUseItem, ItemData = itemData
                };

                this.itemCollectionItemModels.Add(model);
            }
        }

        #region MyRegion

        protected virtual void OnUseItem(ItemCollectionItemModel obj)
        {
            // If the item is not owned, do not use it
            if (!this.uiTemplateInventoryDataController.TryGetItemData(obj.ItemData.Id, out var itemData) || itemData.CurrentStatus != UITemplateItemData.Status.Owned) return;

            var tempModel = this.itemCollectionItemModels.Where(x => x.ItemBlueprintRecord.Category.Equals(this.currentCategoryTab)).ToList();

            foreach (var model in tempModel) model.IndexItemUsed = model.IndexItemSelected = obj.ItemIndex;

            // Save the selected item
            this.uiTemplateInventoryDataController.UpdateCurrentSelectedItem(this.currentCategoryTab, obj.ItemData.Id);
            this.OnUsedItem(obj.ItemData);

            this.View.decorationItemAdapter.Refresh();
            this.uiTemplateInventoryDataController.UpdateCurrentSelectedItem(obj.ItemBlueprintRecord.Category, obj.ItemBlueprintRecord.Id);
            this.uiTemplateDecorationManager.ChangeItem(obj.ItemBlueprintRecord.Category, obj.ItemBlueprintRecord.ImageAddress);
        }

        protected virtual void OnUsedItem(UITemplateItemData itemData)
        {
        }

        private void OnSelectItem(ItemCollectionItemModel obj)
        {
            var tempModel = this.itemCollectionItemModels.Where(x => x.ItemBlueprintRecord.Category.Equals(this.currentCategoryTab)).ToList();

            foreach (var model in tempModel) model.IndexItemSelected = obj.ItemIndex;

            // Save the selected item
            this.OnSelectedItem(obj.ItemData);

            this.View.decorationItemAdapter.Refresh();
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
                case UITemplateItemData.UnlockType.IAP:
                    this.BuyWithIAP(obj);

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

        #endregion

        #region Buy Item

        private void BuyWithDailyReward(ItemCollectionItemModel obj)
        {
            _ = this.uiTemplateDailyRewardService.ShowDailyRewardPopupAsync(true);
        }

        private void BuyWithLuckySpin(ItemCollectionItemModel obj)
        {
            this.uiTemplateLuckySpinServices.OpenLuckySpin();
        }

        private void BuyWithSoftCurrency(ItemCollectionItemModel obj)
        {
            var currentCoin = this.uiTemplateInventoryDataController.GetCurrencyValue();

            if (currentCoin < obj.ShopBlueprintRecord.Price)
            {
                Debug.Log($"Not Enough {obj.ShopBlueprintRecord.CurrencyID}\nCurrent: {currentCoin}, Needed: {obj.ShopBlueprintRecord.Price}");

                return;
            }

            this.uiTemplateInventoryDataController.AddCurrency(-obj.ShopBlueprintRecord.Price, obj.ShopBlueprintRecord.CurrencyID);
            this.BuyItemCompleted(obj);
        }

        private void BuyWithAds(ItemCollectionItemModel obj)
        {
            this.uiTemplateAdServiceWrapper.ShowRewardedAd(AdPlacement, () =>
            {
                this.BuyItemCompleted(obj);
            });
        }

        private void BuyWithIAP(ItemCollectionItemModel obj)
        {
            this.unityUnityIapServices.BuyProductID(obj.ShopBlueprintRecord.CurrencyID, x =>
            {
                this.BuyItemCompleted(obj);
            });
        }

        private void BuyItemCompleted(ItemCollectionItemModel obj)
        {
            obj.ItemData.CurrentStatus = UITemplateItemData.Status.Owned;
            this.uiTemplateInventoryDataController.AddItemData(obj.ItemData);
            this.uiTemplateInventoryData.CategoryToChosenItem[obj.ItemBlueprintRecord.Category] = obj.ItemBlueprintRecord.Id;
            this.uiTemplateInventoryDataController.UpdateCurrentSelectedItem(obj.ItemBlueprintRecord.Category, obj.ItemBlueprintRecord.Id);
            this.OnSelectItem(obj);
        }

        #endregion

        private async UniTask OnActiveTab(string categoryId, bool forceRefresh = false)
        {
            if (this.currentCategoryTab.Equals(categoryId) && !forceRefresh) return;
            foreach (var (key, value) in this.idToCategoryTab)
            {
                value.SetActive(key.Equals(categoryId));
            }

            this.currentCategoryTab = categoryId;
            await this.BindDataToAdapter();
        }
    }
}