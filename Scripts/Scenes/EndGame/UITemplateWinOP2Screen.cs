namespace TheOneStudio.UITemplate.UITemplate.Scenes.EndGame
{
    using Core.AdsServices;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using UnityEngine.Scripting;
    using UnityEngine.UI;

    public class UITemplateWinOP2Screen : BaseEndGameScreenView
    {
        public Button                 btnX2Reward;
        public UITemplateCurrencyView currencyView;
    }

    [ScreenInfo(nameof(UITemplateWinOP2Screen))]
    public class UITemPlateWinOp2ScreenPresenter : BaseEndGameScreenPresenter<UITemplateWinOP2Screen>
    {
        [Preserve]
        public UITemPlateWinOp2ScreenPresenter(
            SignalBus                  signalBus,
            ILogService                logger,
            UITemplateAdServiceWrapper uiTemplateAdService,
            UITemplateSoundServices    soundServices
        ) : base(signalBus, logger, uiTemplateAdService, soundServices)
        {
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.btnX2Reward.onClick.AddListener(this.OnX2Reward);
        }

        public override UniTask BindData()
        {
            base.BindData();
            this.UITemplateAdService.ShowMREC<UITemPlateWinOp2ScreenPresenter>(AdViewPosition.Centered);
            this.SoundServices.PlaySoundWin();
            this.UITemplateAdService.HideBannerAd();
            return UniTask.CompletedTask;
        }

        protected virtual void OnX2Reward()
        {
            this.UITemplateAdService.ShowRewardedAd("x2Reward", this.AfterWatchAdsX2Reward);
        }

        protected virtual void AfterWatchAdsX2Reward()
        {
        }

        protected override void OnClickNext()
        {
        }

        public override void Dispose()
        {
            base.Dispose();
            this.UITemplateAdService.HideMREC(AdViewPosition.Centered);
            this.UITemplateAdService.ShowBannerAd();
        }
    }
}