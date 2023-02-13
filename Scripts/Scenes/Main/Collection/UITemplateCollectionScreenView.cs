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

        #endregion
        
        private enum CollectionTab
        {
            Character,
            Item
        }

        private List<UITemplateCollectionItemModel> collectionItemModels = new();

        public UITemplateCollectionScreenPresenter(SignalBus signalBus, IScreenManager screenManager, DiContainer diContainer,
            UITemplateShopBlueprint shopBlueprint) : base(signalBus)
        {
            this.screenManager  = screenManager;
            this.diContainer    = diContainer;
            this.shopBlueprint = shopBlueprint;
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
            this.BindDataForCollection(CollectionTab.Character);
        }
        
        

        private void OnClickItem()
        {
            this.BindDataForCollection(CollectionTab.Item);
            this.ConfigBtnStatus(false, true);
            Debug.Log("click item");
        }

        private void OnClickCharacters()
        {
            this.BindDataForCollection(CollectionTab.Character);
            this.ConfigBtnStatus(true, false);
            Debug.Log("click character");
        }

        private void OnClickHome() { this.screenManager.OpenScreen<UITemplateHomeSimpleScreenPresenter>(); }

        private void OnClickWatchAds() { }

        private void BindDataForCollection(CollectionTab collectionTab)
        {
            switch (collectionTab)
            {
                case CollectionTab.Character:
                    this.ConfigBtnStatus(true, false);
                    this.BindDataForCharacters();
                    break;
                case CollectionTab.Item:
                    this.ConfigBtnStatus(false, true);
                    this.BindDataForItems();
                    break;
            }
        }

        private void BindDataForCharacters()
        {
            // this.collectionItemModels.Clear();
            // this.collectionItemModels = this.shopBlueprint.Values.Where(item => item.Category.Equals(CollectionTab.Character.ToString()))
            //     .Select(item => new UITemplateCollectionItemModel(item, CollectionTab.Character))
            //     .ToList();
        }

        private void BindDataForItems()
        {
            // items
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