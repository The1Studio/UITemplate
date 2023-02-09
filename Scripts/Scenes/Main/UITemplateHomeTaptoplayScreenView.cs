namespace UITemplate.Scripts.Scenes.Main
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using UITemplate.Scripts.Scenes.Popups;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateHomeTaptoplayScreenView : BaseView
    {
        public Button                      TaptoplayButton;
        public UITemplateCurrencyView      CoinText;
        public UITemplateSettingButtonView SettingButtonView;
    }

    [ScreenInfo(nameof(UITemplateHomeTaptoplayScreenView))]
    public class UITemplateHomeTaptoplayScreenPresenter : BaseScreenPresenter<UITemplateHomeTaptoplayScreenView>
    {
        private readonly IScreenManager screenManager;
        private readonly DiContainer    diContainer;

        public UITemplateHomeTaptoplayScreenPresenter(SignalBus signalBus, IScreenManager screenManager, DiContainer diContainer) : base(signalBus)
        {
            this.screenManager = screenManager;
            this.diContainer   = diContainer;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.TaptoplayButton.onClick.AddListener(this.OnClickTaptoplayBtn);
            this.diContainer.Inject(this.View.SettingButtonView);
        }

        public override void BindData() { this.View.CoinText.Subscribe(this.SignalBus); }

        private void OnClickTaptoplayBtn()
        {
            // load play screen
        }

        public override void Dispose()
        {
            base.Dispose();
            this.View.CoinText.Unsubscribe(this.SignalBus);
        }
    }
}