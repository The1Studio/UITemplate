namespace TheOneStudio.UITemplate.UITemplate.Scenes.EndGame
{
    using Core.AdsServices;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateWinOP2Screen : BaseEndGameScreenView
    {
        public Button                 btnX2Reward;
        public UITemplateCurrencyView currencyView;
    }

    [ScreenInfo(nameof(UITemplateWinOP2Screen))]
    public class UITemPlateWinOp2ScreenPresenter : BaseEndGameScreenPresenter<UITemplateWinOP2Screen>
    {
        #region inject

        private readonly UITemplateInventoryDataController uiTemplateInventoryDataController;
        private readonly DiContainer                       diContainer;

        #endregion

        public UITemPlateWinOp2ScreenPresenter(
            SignalBus                         signalBus,
            ILogService                       logger,
            UITemplateAdServiceWrapper        uiTemplateAdService,
            UITemplateSoundServices           soundServices,
            UITemplateInventoryDataController uiTemplateInventoryDataController,
            DiContainer                       diContainer
        ) : base(signalBus, logger, uiTemplateAdService, soundServices)
        {
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
            this.diContainer                       = diContainer;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.btnX2Reward.onClick.AddListener(this.OnX2Reward);
            if (this.View.currencyView != null)
            {
                this.diContainer.Inject(this.View.currencyView);
            }
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