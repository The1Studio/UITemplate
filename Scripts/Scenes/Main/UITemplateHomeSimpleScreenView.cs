namespace UITemplate.Scripts.Scenes.Main
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using UITemplate.Scripts.Scenes.Popups;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateHomeSimpleScreenView : BaseView
    {
        public Button                 PlayButton;
        public Button                 LevelButton;
        public UITemplateCurrencyText CoinText;
    }

    [ScreenInfo(nameof(UITemplateHomeSimpleScreenView))]
    public class UITemplateHomeSimpleScreenPresenter : BaseScreenPresenter<UITemplateHomeSimpleScreenView>
    {
        public UITemplateHomeSimpleScreenPresenter(SignalBus signalBus) : base(signalBus) { }

        protected override async void OnViewReady()
        {
            base.OnViewReady();
            await this.OpenViewAsync();
            this.View.PlayButton.onClick.AddListener(this.OnClickPlay);
            this.View.LevelButton.onClick.AddListener(this.OnClickLevel);
        }
        private void OnClickLevel() { }
        private void OnClickPlay()  { }

        public override void BindData()
        {
            this.View.CoinText.Subscribe(this.SignalBus);
        }

        public override void Dispose()
        {
            base.Dispose();
            this.View.CoinText.Unsubscribe(this.SignalBus);
        }
    }
}