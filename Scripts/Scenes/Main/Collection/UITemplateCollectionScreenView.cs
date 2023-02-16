namespace UITemplate.Scripts.Scenes.Main.Collection
{
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using UITemplate.Scripts.Blueprints;
    using UITemplate.Scripts.Models;
    using UITemplate.Scripts.Scenes.Main.Collection.Elements;
    using UITemplate.Scripts.Scenes.Popups;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateCollectionScreenView : BaseView
    {
        public UITemplateCurrencyView     CoinText;
        public Button                     HomeButton;
        public UITemplateOnOffButton      CharactersButton;
        public UITemplateOnOffButton      ItemsButton;
        public Button                     WatchAdsButton;
        public CharacterCollectionAdapter CharacterCollectionAdapter;
        public ItemCollectionAdapter      ItemCollectionAdapter;
    }

    [ScreenInfo(nameof(UITemplateCollectionScreenView))]
    public class UITemplateCollectionScreenPresenter : BaseScreenPresenter<UITemplateCollectionScreenView>
    {
        #region Inject

        private readonly SignalBus               signalBus;
        private readonly IScreenManager          screenManager;
        private readonly DiContainer             diContainer;
        private readonly UITemplateShopBlueprint shopBlueprint;
        private readonly UITemplateUserData      userData;

        #endregion

        private       List<CharacterCollectionItemModel> characterLists = new();
        private       List<ItemCollectionItemModel>      itemLists      = new();
        private const string                             CatCharacter   = "Character";
        private const string                             CatItem        = "Item";

        public UITemplateCollectionScreenPresenter(SignalBus signalBus, IScreenManager screenManager, DiContainer diContainer,
            UITemplateShopBlueprint shopBlueprint, UITemplateUserData userData) : base(signalBus)
        {
            this.signalBus     = signalBus;
            this.screenManager = screenManager;
            this.diContainer   = diContainer;
            this.shopBlueprint = shopBlueprint;
            this.userData      = userData;
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
            this.View.CoinText.Subscribe(this.SignalBus);
            this.PrePareCharacterModel(this.characterLists);
            this.PrePareItemModel(this.itemLists);
            this.SelectTabCategory(CatCharacter);
        }

        private void PrePareCharacterModel(List<CharacterCollectionItemModel> source)
        {
            source.Clear();
            for (var i = 0; i < this.shopBlueprint.Values.Count; i++)
            {
                var currentElement = this.shopBlueprint.Values.ElementAt(i);

                if (!currentElement.Category.Equals(CatCharacter)) continue;
                var model = new CharacterCollectionItemModel()
                {
                    Index            = i,
                    ItemData         = this.userData.ShopData.GetItemData(currentElement.Name),
                    Category         = CatCharacter,
                    OnBuy            = this.OnBuyCharacter,
                    OnSelected       = this.OnSelectedCharacter,
                    OnNotEnoughMoney = this.OnNotEnoughMoney,
                };
                source.Add(model);
            }
        }

        private void OnNotEnoughMoney()
        {
            // show popup not enough money here
        }

        private void OnBuyCharacter(CharacterCollectionItemModel obj)
        {
            obj.ItemData.CurrentStatus                                   = ItemData.Status.Owned;
            this.userData.UserPackageData.CurrentSelectCharacterId.Value = obj.ItemData.BlueprintRecord.Name;
            this.userData.ShopData.UpdateStatusItemData(obj.ItemData.BlueprintRecord.Name, ItemData.Status.Owned);
            // update payment coin here

            this.View.CharacterCollectionAdapter.Refresh();
        }

        private void OnSelectedCharacter(CharacterCollectionItemModel obj)
        {
            this.userData.UserPackageData.CurrentSelectCharacterId.Value = obj.ItemData.BlueprintRecord.Name;
            this.View.CharacterCollectionAdapter.Refresh();
        }

        private void PrePareItemModel(List<ItemCollectionItemModel> source)
        {
            source.Clear();
            for (var i = 0; i < this.shopBlueprint.Values.Count; i++)
            {
                var currentElement = this.shopBlueprint.Values.ElementAt(i);

                if (!currentElement.Category.Equals(CatItem)) continue;
                var model = new ItemCollectionItemModel()
                {
                    Index            = i,
                    ItemData         = this.userData.ShopData.GetItemData(currentElement.Name),
                    Category         = CatItem,
                    OnBuy            = this.OnBuyItem,
                    OnSelected       = this.OnSelectedItem,
                    OnNotEnoughMoney = this.OnNotEnoughMoney,
                };
                source.Add(model);
            }
        }

        private void OnBuyItem(ItemCollectionItemModel obj)
        {
            obj.ItemData.CurrentStatus                              = ItemData.Status.Owned;
            this.userData.UserPackageData.CurrentSelectItemId.Value = obj.ItemData.BlueprintRecord.Name;
            this.userData.ShopData.UpdateStatusItemData(obj.ItemData.BlueprintRecord.Name, ItemData.Status.Owned);
            // update payment coin here

            this.View.ItemCollectionAdapter.Refresh();
        }

        private void OnSelectedItem(ItemCollectionItemModel obj)
        {
            this.userData.UserPackageData.CurrentSelectItemId.Value = obj.ItemData.BlueprintRecord.Name;
            this.View.ItemCollectionAdapter.Refresh();
        }

        private async void SelectTabCategory(string categoryTab)
        {
            if (categoryTab.Equals(CatItem))
            {
                await this.View.ItemCollectionAdapter.InitItemAdapter(this.itemLists, this.diContainer);
            }
            else
            {
                await this.View.CharacterCollectionAdapter.InitItemAdapter(this.characterLists, this.diContainer);
            }

            this.View.ItemCollectionAdapter.gameObject.SetActive(categoryTab.Equals(CatItem));
            this.View.CharacterCollectionAdapter.gameObject.SetActive(categoryTab.Equals(CatCharacter));
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

        private void OnClickHome() { this.screenManager.OpenScreen<UITemplateHomeSimpleScreenPresenter>(); }

        private void OnClickWatchAds() { }

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
    }
}