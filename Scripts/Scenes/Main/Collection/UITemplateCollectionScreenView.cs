namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.Collection
{
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.Collection.Elements;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateCollectionScreenView : BaseView
    {
        public UITemplateCurrencyView CoinText;
        public Button                 HomeButton;
        public UITemplateOnOffButton  CharactersButton;
        public UITemplateOnOffButton  ItemsButton;
        public Button                 WatchAdsButton;
        public ItemCollectionAdapter  ItemCollectionAdapter;
    }

    [ScreenInfo(nameof(UITemplateCollectionScreenView))]
    public class UITemplateCollectionScreenPresenter : UITemplateBaseScreenPresenter<UITemplateCollectionScreenView>
    {
        private const string CatCharacter = "Character";
        private const string CatItem      = "Item";

        private readonly List<ItemCollectionItemModel> itemLists = new();

        public UITemplateCollectionScreenPresenter(SignalBus signalBus, IScreenManager screenManager, DiContainer diContainer, UITemplateShopBlueprint shopBlueprint, UITemplateInventoryDataController uiTemplateInventoryDataController) :
            base(signalBus)
        {
            this.screenManager                     = screenManager;
            this.diContainer                       = diContainer;
            this.shopBlueprint                     = shopBlueprint;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.HomeButton.onClick.AddListener(this.OnClickHome);
            this.View.WatchAdsButton.onClick.AddListener(this.OnClickWatchAds);
            this.View.CharactersButton.Button.onClick.AddListener(this.OnClickCharacters);
            this.View.ItemsButton.Button.onClick.AddListener(this.OnClickItem);
        }

        public override void BindData()
        {
            this.View.CoinText.Subscribe(this.SignalBus, this.uiTemplateInventoryDataController.GetCurrency().Value);
            this.GetItemDataList(this.itemLists);
            this.SelectTabCategory(CatCharacter);
        }

        private void OnNotEnoughMoney()
        {
            // show popup not enough money here
        }

        private void GetItemDataList(List<ItemCollectionItemModel> source)
        {
            source.Clear();
            for (var i = 0; i < this.shopBlueprint.Values.Count; i++)
            {
                var currentElement = this.shopBlueprint.Values.ElementAt(i);

                if (!currentElement.Category.Equals(CatItem)) continue;
                var model = new ItemCollectionItemModel
                {
                    Index              = i,
                    UITemplateItemData = this.uiTemplateInventoryDataController.GetItemData(currentElement.Name),
                    Category           = CatItem,
                    OnBuy              = this.OnBuyItem,
                    OnSelected         = this.OnSelectedItem,
                    OnNotEnoughMoney   = this.OnNotEnoughMoney
                };
                source.Add(model);
            }
        }

        private void OnBuyItem(ItemCollectionItemModel obj)
        {
            obj.UITemplateItemData.CurrentStatus = UITemplateItemData.Status.Owned;
            // this.userData.InventoryData.CurrentSelectItemId.Value = obj.UITemplateItemData.BlueprintRecord.Name;
            this.uiTemplateInventoryDataController.UpdateStatusItemData(obj.UITemplateItemData.BlueprintRecord.Name, UITemplateItemData.Status.Owned);
            // update payment coin here

            this.View.ItemCollectionAdapter.Refresh();
        }

        private void OnSelectedItem(ItemCollectionItemModel obj)
        {
            // this.userData.UserPackageData.CurrentSelectItemId.Value = obj.UITemplateItemData.BlueprintRecord.Name;
            this.View.ItemCollectionAdapter.Refresh();
        }

        private async void SelectTabCategory(string categoryTab)
        {
            if (categoryTab.Equals(CatItem)) await this.View.ItemCollectionAdapter.InitItemAdapter(this.itemLists, this.diContainer);

            // await this.View.CharacterCollectionAdapter.InitItemAdapter(this.characterLists, this.diContainer);
            this.View.ItemCollectionAdapter.gameObject.SetActive(categoryTab.Equals(CatItem));
            // this.View.CharacterCollectionAdapter.gameObject.SetActive(categoryTab.Equals(CatCharacter));
        }

        private void OnClickItem()
        {
            this.SelectTabCategory(CatItem);
            this.ConfigBtnStatus(false, true);
        }

        private void OnClickCharacters()
        {
            this.SelectTabCategory(CatCharacter);
            this.ConfigBtnStatus(true, false);
        }

        private void OnClickHome()
        {
            this.screenManager.OpenScreen<UITemplateHomeSimpleScreenPresenter>();
        }

        private void OnClickWatchAds()
        {
        }

        private void ConfigBtnStatus(bool isCharacterActive, bool isItemActive)
        {
            this.View.CharactersButton.SetOnOff(isCharacterActive);
            this.View.ItemsButton.SetOnOff(isItemActive);
        }

        public override void Dispose()
        {
            base.Dispose();
            this.View.CoinText.Unsubscribe(this.SignalBus);
        }

        #region Inject

        private readonly IScreenManager                    screenManager;
        private readonly DiContainer                       diContainer;
        private readonly UITemplateShopBlueprint           shopBlueprint;
        private readonly UITemplateInventoryDataController uiTemplateInventoryDataController;

        #endregion
    }
}