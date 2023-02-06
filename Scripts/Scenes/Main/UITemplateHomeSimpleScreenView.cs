namespace UITemplate.Scripts.Scenes.Main
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using UITemplate.Scripts.Scenes.Play.End;
    using UITemplate.Scripts.Scenes.Popups;
    using UnityEngine;
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
        
        public UITemplateHomeSimpleScreenPresenter(SignalBus signalBus, IScreenManager screenManager, DiContainer diContainer) : base(signalBus)
        {
            this.diContainer   = diContainer;
            this.screenManager = screenManager;
        }

        protected override async void OnViewReady()
        {
            base.OnViewReady();
            await this.OpenViewAsync();
            this.diContainer.Inject(this.View.SettingButtonView);
            this.View.PlayButton.onClick.AddListener(this.OnClickPlay);
            this.View.LevelButton.onClick.AddListener(this.OnClickLevel);
        }

        private void OnClickLevel()
        {
            // this.screenManager.OpenScreen<UITemplateWinScreenPresenter, UITemplateWinScreenModel>(new UITemplateWinScreenModel(2, "Character_Sample05_l", 25, 50));
        }

        private void OnClickPlay()
        {
            // this.screenManager.OpenScreen<UITemplateWinScreenPresenter, UITemplateWinScreenModel>(new UITemplateWinScreenModel(1, "Character_Sample05_l", 0, 25));
        }

        public override void BindData() { this.View.CoinText.Subscribe(this.SignalBus); }

        public override void Dispose()
        {
            base.Dispose();
            this.View.CoinText.Unsubscribe(this.SignalBus);
        }
    }
}