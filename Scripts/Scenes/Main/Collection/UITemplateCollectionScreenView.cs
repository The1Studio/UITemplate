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
        public UITemplateCollectionAdapter CollectionAdapter;
    }

    [ScreenInfo(nameof(UITemplateCollectionScreenView))]
    public class UITemplateCollectionScreenPresenter : BaseScreenPresenter<UITemplateCollectionScreenView>
    {
        #region Inject

        private readonly IScreenManager          screenManager;
        private readonly DiContainer             diContainer;
        private readonly UITemplateShopBlueprint shopBlueprint;
        private readonly UITemplateUserData      userData;

        #endregion
        
        private enum CollectionTab
        {
            Characters,
            Items
        }

        private List<UITemplateCollectionItemModel> collectionItemModels = new();

        public UITemplateCollectionScreenPresenter(SignalBus signalBus, IScreenManager screenManager, DiContainer diContainer,
            UITemplateShopBlueprint shopBlueprint, UITemplateUserData userData) : base(signalBus)
        {
            this.screenManager = screenManager;
            this.diContainer   = diContainer;
            this.shopBlueprint = shopBlueprint;
            this.userData     = userData;
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
            this.View.CollectionAdapter.InitItemAdapter(this.collectionItemModels, this.diContainer);
            this.View.CoinText.Subscribe(this.SignalBus);
            this.InitDataForCollection(CollectionTab.Characters);
        }
        
        private void OnClickItem()
        {
            this.InitDataForCollection(CollectionTab.Items);
            this.ConfigBtnStatus(false, true);
            Debug.Log("click item");
        }

        private void OnClickCharacters()
        {
            this.InitDataForCollection(CollectionTab.Characters);
            this.ConfigBtnStatus(true, false);
            Debug.Log("click character");
        }

        private void OnClickHome() { this.screenManager.OpenScreen<UITemplateHomeSimpleScreenPresenter>(); }

        private void OnClickWatchAds() { }

        private void InitDataForCollection(CollectionTab collectionTab)
        {
            switch (collectionTab)
            {
                case CollectionTab.Characters:
                    this.ConfigBtnStatus(true, false);
                    this.InitDataForCharacters();
                    break;
                case CollectionTab.Items:
                    this.ConfigBtnStatus(false, true);
                    this.InitDataForItems();
                    break;
            }
        }

        private void InitDataForCharacters()
        {
            this.collectionItemModels.Clear();
            this.collectionItemModels = this.shopBlueprint.Values.Where(item => item.Category.Equals(CollectionTab.Characters.ToString()))
                .Select(item => new UITemplateCollectionItemModel(this.userData.ShopData.GetItemData(item.Name), CollectionTab.Characters.ToString()))
                .ToList();
        }

        private void InitDataForItems()
        {
            this.collectionItemModels.Clear();
            this.collectionItemModels = this.shopBlueprint.Values.Where(item => item.Category.Equals(CollectionTab.Items.ToString()))
                .Select(item => new UITemplateCollectionItemModel(this.userData.ShopData.GetItemData(item.Name), CollectionTab.Items.ToString()))
                .ToList();
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
    }
}