#if FALCON
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Falcon
{
    using System;
    using Core.AdsServices.Signals;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using global::Falcon.FalconAnalytics.Scripts.Enum;
    using global::Falcon.FalconAnalytics.Scripts.Models.Messages.PreDefines;
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
            new FLevelLog(obj.Level, "", obj.IsWin ? LevelStatus.Pass : LevelStatus.Fail, TimeSpan.FromSeconds(obj.Time)).Send();
        }
        

        private void OnLevelStarted(LevelStartedSignal obj) { }

        private void OnScreenShow(ScreenShowSignal obj)
        {
            var currentScreen = this.screenManager.CurrentActiveScreen.Value;
            new FActionLog($"ShowScreen_{currentScreen.GetType().Name}").Send();
        }

        private void OnUpdateCurrency(OnUpdateCurrencySignal obj)
        {
            new FResourceLog(obj.Amount > 0 ? FlowType.Source : FlowType.Sink, "currency", obj.Id, obj.Id, Math.Abs(obj.Amount)).Send();
        }

        private void OnShowRewardedAd(RewardedAdCalledSignal obj)
        {
            new FAdLog(AdType.Reward, this.screenManager.CurrentActiveScreen.Value.GetType().Name).Send();
        }

        private void OnShowInterstitialAd(InterstitialAdCalledSignal obj)
        {
            new FAdLog(AdType.Interstitial, this.screenManager.CurrentActiveScreen.Value.GetType().Name).Send();
        }

        private void OnPurchaseComplete(OnIAPPurchaseSuccessSignal obj)
        {
            var productData = this.iapServices.GetProductData(obj.Product.Id);

            throw new NotImplementedException();
            //Get transaction id and Implement this
            // var transactionID = "obj.PurchasedProduct.transactionID";
            // new FInAppLog(obj.Product.Id, productData.Price, productData.CurrencyCode, "", transactionID, 
            //     this.screenManager.CurrentActiveScreen.Value.ToString()).Send();
        }
    }
}
#endif