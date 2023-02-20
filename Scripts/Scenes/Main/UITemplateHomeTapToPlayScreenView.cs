namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
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
        private readonly IScreenManager screenManager;
        private readonly DiContainer    diContainer;

        public UITemplateHomeTapToPlayScreenPresenter(SignalBus signalBus, IScreenManager screenManager, DiContainer diContainer) : base(signalBus)
        {
            this.screenManager = screenManager;
            this.diContainer   = diContainer;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.OpenViewAsync();
            this.View.TaptoplayButton.onClick.AddListener(this.OnClickTapToPlayButton);
            this.View.ShopButton.onClick.AddListener(this.OnClickShopButton);
            this.diContainer.Inject(this.View.SettingButtonView);
        }

        public override void BindData()
        {
            this.View.CoinText.Subscribe(this.SignalBus);
        }
        
        protected virtual void OnClickShopButton()      { }
        
        protected virtual void OnClickTapToPlayButton() { }
        
        public override void Dispose()
        {
            base.Dispose();
            this.View.CoinText.Unsubscribe(this.SignalBus);
        }
    }
}