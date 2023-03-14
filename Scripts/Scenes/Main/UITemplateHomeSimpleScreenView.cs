namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateHomeSimpleScreenView : BaseView
    {
        public Button                      PlayButton;
        public Button                      LevelButton;
        public UITemplateCurrencyView      CoinText;
        public UITemplateSettingButtonView SettingButtonView;
    }

    [ScreenInfo(nameof(UITemplateHomeSimpleScreenView))]
    public class UITemplateHomeSimpleScreenPresenter : UITemplateBaseScreenPresenter<UITemplateHomeSimpleScreenView>
    {
        public UITemplateHomeSimpleScreenPresenter(SignalBus                         signalBus, UITemplateInventoryData inventoryData, DiContainer diContainer, IScreenManager screenManager,
                                                   UITemplateInventoryDataController uiTemplateInventoryDataController) : base(signalBus)
        {
            this.inventoryData                     = inventoryData;
            this.diContainer                       = diContainer;
            this.ScreenManager                     = screenManager;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
        }

        protected override async void OnViewReady()
        {
            base.OnViewReady();
            await this.OpenViewAsync();
            this.diContainer.Inject(this.View.SettingButtonView);
            this.View.PlayButton.onClick.AddListener(this.OnClickPlay);
            this.View.LevelButton.onClick.AddListener(this.OnClickLevel);
        }

        protected virtual void OnClickLevel()
        {
            this.ScreenManager.OpenScreen<UITemplateLevelSelectScreenPresenter>();
        }

        protected virtual void OnClickPlay()
        {
        }

        public override void BindData()
        {
            this.View.CoinText.Subscribe(this.SignalBus, this.uiTemplateInventoryDataController.GetCurrency().Value);
        }

        public override void Dispose()
        {
            base.Dispose();
            this.View.CoinText.Unsubscribe(this.SignalBus);
        }

        #region inject

        private readonly   UITemplateInventoryData           inventoryData;
        private readonly   DiContainer                       diContainer;
        protected readonly IScreenManager                    ScreenManager;
        protected readonly UITemplateInventoryDataController uiTemplateInventoryDataController;

        #endregion
    }
}