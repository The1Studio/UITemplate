namespace TheOneStudio.UITemplate.UITemplate.Scenes.EndGame
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateWinOP2Screen : BaseEndGameScreenView
    {
        public Button btnX2Reward;
    }

    [ScreenInfo(nameof(UITemplateWinOP2Screen))]
    public class UITemPlateWinOp2ScreenPresenter : BaseEndGameScreenPresenter<UITemplateWinOP2Screen>
    {
        public UITemPlateWinOp2ScreenPresenter(SignalBus signalBus, UITemplateAdServiceWrapper uiTemplateAdService, UITemplateSoundService soundService) :
            base(signalBus, uiTemplateAdService, soundService)
        {
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.btnX2Reward.onClick.AddListener(this.OnX2Reward);
        }

        public override void BindData()
        {
            base.BindData();
            this.SoundService.PlaySoundWin();
        }

        protected virtual void OnX2Reward() { this.UITemplateAdService.ShowRewardedAd("x2Reward", this.AfterWatchAdsX2Reward); }

        protected virtual void AfterWatchAdsX2Reward() { }

        protected override void OnClickNext() { }
    }
}