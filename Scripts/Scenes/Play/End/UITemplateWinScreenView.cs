namespace UITemplate.Scripts.Scenes.Play.End
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.Utilities.LogService;
    using UITemplate.Scripts.Scenes.Main;
    using UITemplate.Scripts.Scenes.Popups;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateWinScreenModel
    {
        public int StarRate;
        public UITemplateWinScreenModel(int starRate) { this.StarRate = starRate; }
    }

    public class UITemplateWinScreenView : BaseView
    {
        public Button                 HomeButton;
        public Button                 ReplayEndgameButton;
        public Button                 NextEndgameButton;
        public UITemplateCurrencyView CoinText;
        public UITemplateStarRateView starRateView;
    }

    [ScreenInfo(nameof(UITemplateWinScreenView))]
    public class UITemplateWinScreenPresenter : BaseScreenPresenter<UITemplateWinScreenView, UITemplateWinScreenModel>
    {
        private readonly IScreenManager screenManager;
        public UITemplateWinScreenPresenter(SignalBus signalBus, ILogService logService, IScreenManager screenManager) : base(signalBus, logService) { this.screenManager = screenManager; }

        protected override async void OnViewReady()
        {
            base.OnViewReady();
            await this.OpenViewAsync();
            this.View.HomeButton.onClick.AddListener(this.OnClickHome);
            this.View.ReplayEndgameButton.onClick.AddListener(this.OnClickReplay);
            this.View.NextEndgameButton.onClick.AddListener(this.OnClickNext);
        }

        public override void BindData(UITemplateWinScreenModel model)
        {
            this.View.CoinText.Subscribe(this.SignalBus);
            this.View.starRateView.SetStarRate(this.Model.StarRate);
        }

        private void OnClickHome()   { this.screenManager.OpenScreen<UITemplateHomeSimpleScreenPresenter>(); }
        private void OnClickReplay() { }
        private void OnClickNext()   { }

        public override void Dispose()
        {
            base.Dispose();
            this.View.CoinText.Unsubscribe(this.SignalBus);
        }
    }
}