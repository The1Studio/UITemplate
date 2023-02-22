namespace TheOneStudio.UITemplate.UITemplate.Scenes.EndGame
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using TheOneStudio.UITemplate.UITemplate.Interfaces;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateWinOP2Screen : BaseEndGameScreenView
    {
        public Button btnX2Reward;
    }

    [ScreenInfo(nameof(UITemplateWinOP2Screen))]
    public class UITemPlateWinOp2ScreenPresenter : BaseEndGameScreenPresenter<UITemplateWinOP2Screen>
    {
        public UITemPlateWinOp2ScreenPresenter(SignalBus signalBus, IAdsSystem adsSystem) : base(signalBus, adsSystem) { }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.btnX2Reward.onClick.AddListener(this.OnX2Reward);
        }

        private void OnX2Reward() { this.AdsSystem.ShowRewardedVideo(() => { }); }

        protected override void OnClickNext() { }
    }
}