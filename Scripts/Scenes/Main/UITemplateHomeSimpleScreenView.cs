namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
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

        private readonly DiContainer    diContainer;
        private readonly IScreenManager screenManager;

        #endregion

        public UITemplateHomeSimpleScreenPresenter(SignalBus signalBus, DiContainer diContainer, IScreenManager screenManager) : base(signalBus)
        {
            this.diContainer   = diContainer;
            this.screenManager = screenManager;
        }

        protected override async void OnViewReady()
        {
            base.OnViewReady();
            await this.OpenViewAsync();
            // this.diContainer.Inject(this.View.SettingButtonView);
            this.View.PlayButton.onClick.AddListener(this.OnClickPlay);
            this.View.LevelButton.onClick.AddListener(this.OnClickLevel);
        }

        protected virtual void OnClickLevel() { this.screenManager.OpenScreen<UITemplateLevelSelectScreenPresenter>(); }

        protected virtual void OnClickPlay() { }

        public override void BindData() { this.View.CoinText.Subscribe(this.SignalBus); }

        public override void Dispose()
        {
            base.Dispose();
            this.View.CoinText.Unsubscribe(this.SignalBus);
        }
    }
}