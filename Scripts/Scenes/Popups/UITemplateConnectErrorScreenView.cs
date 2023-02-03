namespace UITemplate.Scripts.Scenes.Main
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using Zenject;
    using UnityEngine.UI;

    public class UITemplateConnectErrorScreenView : BaseView
    {
        public Button ReconectButton;
    }
    
    [ScreenInfo(nameof(UITemplateConnectErrorScreenView))]
    public class UITemplateConnectErrorPresenter : BaseScreenPresenter<UITemplateConnectErrorScreenView>
    {
        public UITemplateConnectErrorPresenter(SignalBus signalBus) : base(signalBus) { }
        
        protected override async void OnViewReady()
        {
            base.OnViewReady();
            await this.OpenViewAsync();
            this.View.ReconectButton.onClick.AddListener(this.OnClickReconect);
        }
        private void OnClickReconect() { }
        
        public override void BindData()
        {
        }
        
        public override void Dispose()
        {
            base.Dispose();
        }
    }
}