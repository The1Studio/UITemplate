namespace UITemplate.Scripts.Scenes.Main.Collection
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using UITemplate.Scripts.Scenes.Popups;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateCollectionScreenView : BaseView
    {
        public UITemplateCurrencyView CoinText;
        public Button                 HomeButton;
        public UITemplateOnOffButton  CharactersButton;
        public UITemplateOnOffButton  ItemsButton;
        public Button                 WatchAdsButton;
    }

    [ScreenInfo(nameof(UITemplateCollectionScreenView))]
    public class UITemplateCollectionScreenPresenter : BaseScreenPresenter<UITemplateCollectionScreenView>
    {
        private readonly IScreenManager screenManager;

        private enum CollectionTab
        {
            Character,
            Item
        }

        public UITemplateCollectionScreenPresenter(SignalBus signalBus, IScreenManager screenManager) : base(signalBus) { this.screenManager = screenManager; }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.HomeButton.onClick.AddListener(this.OnClickHome);
            this.View.HomeButton.onClick.AddListener(this.OnClickCharacters);
            this.View.HomeButton.onClick.AddListener(this.OnClickItem);
        }

        public override void BindData()
        {
            this.View.CoinText.Subscribe(this.SignalBus);
            this.BindDataForItem(CollectionTab.Character);
        }

        private void OnClickItem()
        {
            this.BindDataForItem(CollectionTab.Item);
            this.ConfigBtnStatus(false, true);
        }

        private void OnClickCharacters()
        {
            this.BindDataForItem(CollectionTab.Character);
            this.ConfigBtnStatus(true, false);
        }

        private void OnClickHome() { this.screenManager.OpenScreen<UITemplateHomeSimpleScreenPresenter>(); }

        private void BindDataForItem(CollectionTab collectionTab)
        {
            switch (collectionTab)
            {
                case CollectionTab.Character:
                    this.ConfigBtnStatus(true, false);
                    break;
                case CollectionTab.Item:
                    this.ConfigBtnStatus(false, true);
                    break;
            }
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