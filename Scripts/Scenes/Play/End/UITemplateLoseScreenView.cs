using UITemplate.Scripts.Scenes.Popups;

namespace UITemplate.Scripts.Scenes.Main
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using Zenject;
    using UnityEngine.UI;
    
    public class UITemplateLoseScreenView : BaseView
    {
        public Button ReplayButton;
        public Button ButtonNextEndgame;
        public Button HomeButton;
        public UITemplateCurrencyText CoinText;
    }
    
    [ScreenInfo(nameof(UITemplateLoseScreenView))]
    
    public  class UITemplateLosePresenter : BaseScreenPresenter<UITemplateLoseScreenView>
    {
        public UITemplateLosePresenter(SignalBus signalBus) : base(signalBus) { }
        
        protected override async void OnViewReady()
        {
            base.OnViewReady();
            await this.OpenViewAsync();
            this.View.HomeButton.onClick.AddListener(this.OnClickHome);
            this.View.ReplayButton.onClick.AddListener(this.OnClickReplay);
            this.View.ButtonNextEndgame.onClick.AddListener(this.OnClickNextEndgame);
        }
        private void OnClickNextEndgame() { }
        private void OnClickReplay() { }
        private void OnClickHome() { }
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