namespace TheOneStudio.UITemplate.UITemplate.Services.BreakAds
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Signals;
    using ServiceImplementation.Configs;
    using Sirenix.OdinInspector;
    using TheOne.Logging;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Scripting;

    public class BreakAdsPopupView : BaseView
    {
        [BoxGroup("Reward Currency")] public RectTransform currencyTransform;
        [BoxGroup("Reward Currency")] public GameObject    goRewardCurrency;
        [BoxGroup("Reward Currency")] public TMP_Text      txtRewardCurrencyValue;
        [BoxGroup("Reward Currency")] public string        currencyValuePattern = "+{0}";
    }

    [PopupInfo(nameof(BreakAdsPopupView), isOverlay: true)]
    public class BreakAdsPopupPresenter : BasePopupPresenter<BreakAdsPopupView>
    {
        #region Inject

        private readonly BreakAdsViewHelper                breakAdsViewHelper;
        private readonly ThirdPartiesConfig                thirdPartiesConfig;

        [Preserve]
        public BreakAdsPopupPresenter(
            SignalBus                         signalBus,
            ILoggerManager                    loggerManager,
            BreakAdsViewHelper                breakAdsViewHelper,
            ThirdPartiesConfig                thirdPartiesConfig
        ) : base(signalBus, loggerManager)
        {
            this.breakAdsViewHelper      = breakAdsViewHelper;
            this.thirdPartiesConfig      = thirdPartiesConfig;
        }

        #endregion

        protected override void OnViewReady()
        {
            this.breakAdsViewHelper.OnViewReady(this.View, this);
        }

        public override UniTask BindData()
        {
            this.SetupUI();
            return this.breakAdsViewHelper.BindData();
        }

        protected virtual void SetupUI()
        {
            this.View.goRewardCurrency.gameObject.SetActive(this.thirdPartiesConfig.AdSettings.IsBreakAdsRewardCurrency);
            if (!this.thirdPartiesConfig.AdSettings.IsBreakAdsRewardCurrency) return;
            this.View.txtRewardCurrencyValue.text = string.Format(this.View.currencyValuePattern, this.thirdPartiesConfig.AdSettings.BreakAdsRewardCurrencyAmount);
        }

        public override void Dispose()
        {
            this.breakAdsViewHelper.Dispose();
        }
    }
}