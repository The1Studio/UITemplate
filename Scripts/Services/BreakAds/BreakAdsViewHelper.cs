namespace TheOneStudio.UITemplate.UITemplate.Services.BreakAds
{
    using System.Threading;
    using Core.AdsServices.Signals;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Signals;
    using ServiceImplementation.Configs;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine.Scripting;

    // Rebind this class to your own view
    public class BreakAdsViewHelper
    {
        public bool IsViewReady => this.BreakAdsPopupPresenter.ScreenStatus == ScreenStatus.Opened;

        protected BreakAdsPopupView       View;
        protected BreakAdsPopupPresenter  BreakAdsPopupPresenter;
        protected CancellationTokenSource Cts;

        #region Inject

        protected readonly SignalBus                         signalBus;
        protected readonly ThirdPartiesConfig                thirdPartiesConfig;
        protected readonly UITemplateInventoryDataController inventoryDataController;

        [Preserve]
        public BreakAdsViewHelper(
            SignalBus                         signalBus,
            ThirdPartiesConfig                thirdPartiesConfig,
            UITemplateInventoryDataController inventoryDataController
        )
        {
            this.signalBus               = signalBus;
            this.thirdPartiesConfig      = thirdPartiesConfig;
            this.inventoryDataController = inventoryDataController;
        }

        #endregion

        public virtual void OnViewReady(BreakAdsPopupView view, BreakAdsPopupPresenter breakAdsPopupPresenter)
        {
            this.View                   = view;
            this.BreakAdsPopupPresenter = breakAdsPopupPresenter;
        }

        public virtual UniTask BindData()
        {
            this.signalBus.Subscribe<InterstitialAdClosedSignal>(this.BreakAdsPopupPresenter.CloseView);
            this.signalBus.Subscribe<InterstitialAdDisplayedFailedSignal>(this.BreakAdsPopupPresenter.CloseView);
            this.signalBus.Subscribe<InterstitialAdDisplayedSignal>(this.BreakAdsPopupPresenter.CloseView);
            this.AutomaticCloseView();

            return UniTask.CompletedTask;
        }

        private async void AutomaticCloseView()
        {
            try
            {
                this.Cts?.Cancel();
                this.Cts = new();
                await UniTask.WaitForSeconds(this.thirdPartiesConfig.AdSettings.TimeDelayCloseBreakAdsPopup, true, cancellationToken: this.Cts.Token);
            }
            finally
            {
                this.BreakAdsPopupPresenter.CloseView();
            }
        }

        public virtual void Dispose()
        {
            if (this.Cts == null) return;
            
            this.RewardAfterWatchedAds();

            this.signalBus.Unsubscribe<InterstitialAdClosedSignal>(this.BreakAdsPopupPresenter.CloseView);
            this.signalBus.Unsubscribe<InterstitialAdDisplayedFailedSignal>(this.BreakAdsPopupPresenter.CloseView);
            this.signalBus.Unsubscribe<InterstitialAdDisplayedSignal>(this.BreakAdsPopupPresenter.CloseView);

            this.Cts.Cancel();
            this.Cts.Dispose();
            this.Cts = null;
        }

        protected virtual void RewardAfterWatchedAds()
        {
            if (!this.thirdPartiesConfig.AdSettings.IsBreakAdsRewardCurrency) return;
            this.inventoryDataController.AddCurrency(this.thirdPartiesConfig.AdSettings.BreakAdsRewardCurrencyAmount, this.thirdPartiesConfig.AdSettings.BreakAdsRewardCurrency, "break_ads", this.View.currencyTransform);
        }
    }
}