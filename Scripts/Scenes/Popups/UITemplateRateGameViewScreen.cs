namespace UITemplate.Scripts.Scenes.Main
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using Zenject;
    using UnityEngine.UI;
    
    public class UITemplateRateGameScreenView : BaseView
    {
        public Button RateButton;
        public Button LaterButton;
    }
    
    [ScreenInfo(nameof(UITemplateRateGameScreenView))]
    public class UITemplateRateGamePresenter : BaseScreenPresenter<UITemplateRateGameScreenView>
    {
        public UITemplateRateGamePresenter(SignalBus signalBus) : base(signalBus) { }
        
        protected override async void OnViewReady()
        {
            base.OnViewReady();
            await this.OpenViewAsync();
            this.View.RateButton.onClick.AddListener(this.OnClickRate);
            this.View.LaterButton.onClick.AddListener(this.OnClickLater);
        }
        private void OnClickLater() { }
        private void OnClickRate()  { }
        
        public override void BindData()
        {
        }
        
        public override void Dispose()
        {
            base.Dispose();
        }
    }
}