namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Models;
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
    public class UITemplateHomeSimpleScreenPresenter : BaseScreenPresenter<UITemplateHomeSimpleScreenView>
    {
        #region inject

        private readonly   UITemplateUserInventoryData userInventoryData;
        private readonly   DiContainer                 diContainer;
        protected readonly IScreenManager              ScreenManager;

        #endregion

        public UITemplateHomeSimpleScreenPresenter(SignalBus signalBus, UITemplateUserInventoryData userInventoryData, DiContainer diContainer, IScreenManager screenManager) : base(signalBus)
        {
            this.userInventoryData = userInventoryData;
            this.diContainer       = diContainer;
            this.ScreenManager     = screenManager;
        }

        protected override async void OnViewReady()
        {
            base.OnViewReady();
            await this.OpenViewAsync();
            this.diContainer.Inject(this.View.SettingButtonView);
            this.View.PlayButton.onClick.AddListener(this.OnClickPlay);
            this.View.LevelButton.onClick.AddListener(this.OnClickLevel);
        }

        protected virtual void OnClickLevel() { this.ScreenManager.OpenScreen<UITemplateLevelSelectScreenPresenter>(); }

        protected virtual void OnClickPlay() { }

        public override void BindData()
        {
            this.View.CoinText.Subscribe(this.SignalBus,
                this.userInventoryData.GetCurrency(UITemplateItemData.UnlockType.SoftCurrency.ToString()).Value);
        }

        public override void Dispose()
        {
            base.Dispose();
            this.View.CoinText.Unsubscribe(this.SignalBus);
        }
    }
}