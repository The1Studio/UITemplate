namespace TheOneStudio.UITemplate.UITemplate.Scenes.EndGame
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateLoseOP2Screen : BaseEndGameScreenView
    {
        public Button btnContinue;
    }

    [ScreenInfo(nameof(UITemplateLoseOP2Screen))]
    public class UITemplateLoseOp2Presenter : BaseEndGameScreenPresenter<UITemplateLoseOP2Screen>
    {
        public UITemplateLoseOp2Presenter(SignalBus signalBus, UITemplateAdServiceWrapper uiTemplateAdService) : base(signalBus, uiTemplateAdService) { }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.btnContinue.onClick.AddListener(this.OnContinue);
        }

        protected virtual void OnContinue() { this.UITemplateAdService.ShowRewardedAd("lose", this.AfterWatchAd); }

        protected virtual void AfterWatchAd() { }

        protected override void OnClickNext() { }
    }
}