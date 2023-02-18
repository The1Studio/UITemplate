namespace UITemplate.Scripts.Scenes.Main
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using UITemplate.Scripts.Scenes.Main.Collection;
    using UITemplate.Scripts.Scenes.Popups;
    using UITemplate.Scripts.Scenes.Utils;
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

        #endregion

        public UITemplateHomeSimpleScreenPresenter(SignalBus signalBus, DiContainer diContainer) : base(signalBus)
        {
            this.diContainer   = diContainer;
        }

        protected override async void OnViewReady()
        {
            base.OnViewReady();
            await this.OpenViewAsync();
            this.diContainer.Inject(this.View.SettingButtonView);
            this.View.PlayButton.onClick.AddListener(this.OnClickPlay);
            this.View.LevelButton.onClick.AddListener(this.OnClickLevel);
        }

        private void OnClickLevel() { }

        private void OnClickPlay() { this.screenManager.OpenScreen<UITemplateCollectionScreenPresenter>(); }

        public override void BindData() { this.View.CoinText.Subscribe(this.SignalBus); }

        public override void Dispose()
        {
            base.Dispose();
            this.View.CoinText.Unsubscribe(this.SignalBus);
        }
    }
}