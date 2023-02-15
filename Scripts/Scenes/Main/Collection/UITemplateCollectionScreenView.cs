namespace UITemplate.Scripts.Scenes.Main.Collection
{
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using UITemplate.Scripts.Blueprints;
    using UITemplate.Scripts.Models;
    using UITemplate.Scripts.Scenes.Popups;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateCollectionScreenView : BaseView
    {
        public UITemplateCurrencyView      CoinText;
        public Button                      HomeButton;
        public UITemplateOnOffButton       CharactersButton;
        public UITemplateOnOffButton       ItemsButton;
        public Button                      WatchAdsButton;
        public UITemplateCollectionAdapter ItemCollectionAdapter;
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

      

        private List<UITemplateCollectionItemModel> characterListItems = new();
        private List<UITemplateCollectionItemModel> itemLists          = new();

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
            // this.PrePareModel(this.characterListItems, CategoryTab.Character);
            // this.PrePareModel(this.itemLists, CategoryTab.Item);
            // this.SelectTabCategory(CategoryTab.Character);
        }

        private void PrePareModel(List<UITemplateCollectionItemModel> source, string category)
        {
            source.Clear();
            for (var i = 0; i < this.shopBlueprint.Values.Count; i++)
            {
                var currentElement = this.shopBlueprint.Values.ElementAt(i);

                if (!currentElement.Category.Equals(category.ToString())) continue;
                var model = new UITemplateCollectionItemModel()
                {
                    Index               = i,
                    ItemData            = this.userData.ShopData.GetItemData(currentElement.Name),
                    Category            = category,
                    OnBuy               = this.OnBuy,
                    OnSelected          = this.OnSelected,
                    OnNotEnoughMoney    = this.OnNotEnoughMoney,
                };
                source.Add(model);
            }
        }

        private async void SelectTabCategory(string categoryTab)
        {
            if (categoryTab == "Character")
            {
                await this.View.ItemCollectionAdapter.InitItemAdapter(this.characterListItems, this.diContainer);
            }
            else
            {
                await this.View.ItemCollectionAdapter.InitItemAdapter(this.itemLists, this.diContainer);
            }
        }

        private void OnBuy(UITemplateCollectionItemModel obj) { }

        private void OnSelected(UITemplateCollectionItemModel obj) { }

        private void OnNotEnoughMoney() { }

        private void OnClickItem()
        {
            this.SelectTabCategory("CategoryTab.Item");
            this.ConfigBtnStatus(false, true);
            Debug.Log("click item");
        }

        private void OnClickCharacters()
        {
            this.SelectTabCategory("CategoryTab.Character");
            this.ConfigBtnStatus(true, false);
            Debug.Log("click character");
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