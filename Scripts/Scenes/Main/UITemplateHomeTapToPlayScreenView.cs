namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.CollectionNew;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Play;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateHomeTapToPlayScreenView : BaseView
    {
        public Button                      TaptoplayButton;
        public Button                      ShopButton;
        public UITemplateCurrencyView      CoinText;
        public UITemplateSettingButtonView SettingButtonView;
    }

    [ScreenInfo(nameof(UITemplateHomeTapToPlayScreenView))]
    public class UITemplateHomeTapToPlayScreenPresenter : BaseScreenPresenter<UITemplateHomeTapToPlayScreenView>
    {
        #region inject

        protected readonly IScreenManager                    ScreenManager;
        protected readonly DiContainer                       DiContainer;
        private readonly   UITemplateInventoryDataController uiTemplateInventoryDataController;

        #endregion
        

        public UITemplateHomeTapToPlayScreenPresenter(SignalBus signalBus, IScreenManager screenManager, DiContainer diContainer, UITemplateInventoryDataController uiTemplateInventoryDataController) : base(signalBus)
        {
            this.ScreenManager                     = screenManager;
            this.DiContainer                       = diContainer;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
        }

        protected override async void OnViewReady()
        {
            base.OnViewReady();
            await this.OpenViewAsync();
            this.View.TaptoplayButton.onClick.AddListener(this.OnClickTapToPlayButton);
            this.View.ShopButton.onClick.AddListener(this.OnClickShopButton);
            this.DiContainer.Inject(this.View.SettingButtonView);
        }

        public override void BindData()
        {
            this.View.CoinText.Subscribe(this.SignalBus,
                this.uiTemplateInventoryDataController.GetCurrency(UITemplateItemData.UnlockType.SoftCurrency.ToString()).Value);
        }

        protected virtual void OnClickShopButton() { this.ScreenManager.OpenScreen<UITemplateNewCollectionScreenPresenter>(); }

        protected virtual void OnClickTapToPlayButton() { this.ScreenManager.OpenScreen<UITemplateGameplayScreenPresenter>(); }

        public override void Dispose()
        {
            base.Dispose();
            this.View.CoinText.Unsubscribe(this.SignalBus);
        }
    }
}