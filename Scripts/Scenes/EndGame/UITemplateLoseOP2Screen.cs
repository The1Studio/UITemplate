namespace TheOneStudio.UITemplate.UITemplate.Scenes.EndGame
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.Utilities;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Services;
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
        private readonly UITemplateSoundService soundService;

        public UITemplateLoseOp2Presenter(SignalBus signalBus, UITemplateAdServiceWrapper uiTemplateAdService, UITemplateSoundService soundService) : base(signalBus, uiTemplateAdService)
        {
            this.soundService = soundService;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.btnContinue.onClick.AddListener(this.OnContinue);
        }

        public override void BindData()
        {
            base.BindData();
            this.soundService.PlaySoundLose();
        }

        protected virtual void OnContinue()
        {
            this.UITemplateAdService.ShowRewardedAd("Lose_Continue", this.AfterWatchAd);
        }

        protected virtual void AfterWatchAd()
        {
        }

        protected override void OnClickNext()
        {
        }
    }
}