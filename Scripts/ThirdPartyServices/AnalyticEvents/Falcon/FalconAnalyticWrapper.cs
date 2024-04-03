#if FALCON
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Falcon
{
    using System;
    using Core.AdsServices.Signals;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using global::Falcon;
    using global::Falcon.FalconAnalytics.Scripts.Enum;
    using ServiceImplementation.IAPServices;
    using ServiceImplementation.IAPServices.Signals;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using Zenject;

    public class FalconAnalyticWrapper : IInitializable
    {
        private readonly SignalBus     signalBus;
        private readonly IIapServices  iapServices;
        private readonly ScreenManager screenManager;

        public FalconAnalyticWrapper(SignalBus signalBus, IIapServices iapServices, ScreenManager screenManager)
        {
            this.signalBus     = signalBus;
            this.iapServices   = iapServices;
            this.screenManager = screenManager;
        }

        public void Initialize()
        {
            this.signalBus.Subscribe<OnIAPPurchaseSuccessSignal>(this.OnPurchaseComplete);
            this.signalBus.Subscribe<InterstitialAdCalledSignal>(this.OnShowInterstitialAd);
            this.signalBus.Subscribe<RewardedAdCalledSignal>(this.OnShowRewardedAd);
            this.signalBus.Subscribe<OnUpdateCurrencySignal>(this.OnUpdateCurrency);
            this.signalBus.Subscribe<ScreenShowSignal>(this.OnScreenShow);
            this.signalBus.Subscribe<LevelStartedSignal>(this.OnLevelStarted);
            this.signalBus.Subscribe<LevelEndedSignal>(this.OnLevelEnded);
        }

        private void OnLevelEnded(LevelEndedSignal obj)
        {
            DWHLog.Instance.LevelLog(obj.Level, obj.Time, 0, "", obj.IsWin ? LevelStatus.Pass : LevelStatus.Fail);
        }
        

        private void OnLevelStarted(LevelStartedSignal obj) { }

        private void OnScreenShow(ScreenShowSignal obj)
        {
            var currentScreen = this.screenManager.CurrentActiveScreen.Value;
            DWHLog.Instance.ActionLog($"ShowScreen_{currentScreen.GetType().Name}");
        }

        private void OnUpdateCurrency(OnUpdateCurrencySignal obj) { DWHLog.Instance.ResourceLog(obj.Amount > 0 ? FlowType.Source : FlowType.Sink, "currency", obj.Id, obj.Id, Math.Abs(obj.Amount)); }

        private void OnShowRewardedAd(RewardedAdCalledSignal obj) { DWHLog.Instance.AdsLog(AdType.Reward, this.screenManager.CurrentActiveScreen.Value.GetType().Name); }

        private void OnShowInterstitialAd(InterstitialAdCalledSignal obj) { DWHLog.Instance.AdsLog(AdType.Interstitial, this.screenManager.CurrentActiveScreen.Value.GetType().Name); }

        private void OnPurchaseComplete(OnIAPPurchaseSuccessSignal obj)
        {
            var productData = this.iapServices.GetProductData(obj.ProductId);

#if THEONE_IAP
            DWHLog.Instance.InAppLog(obj.ProductId, productData.CurrencyCode, productData.Price, obj.PurchasedProduct.transactionID, "",
                this.screenManager.CurrentActiveScreen.Value.ToString());
#endif
        }
    }
}
#endif