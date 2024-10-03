#if FALCON
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Falcon
{
    using System;
    using System.Collections.Generic;
    using Core.AdsServices.Signals;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using global::Falcon.FalconAnalytics.Scripts.Enum;
    using global::Falcon.FalconAnalytics.Scripts.Models.Messages.PreDefines;
    using ServiceImplementation.IAPServices;
    using ServiceImplementation.IAPServices.Signals;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.ABI;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents;
    using GameFoundation.Signals;
    using AdInterClick = TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI.AdInterClick;
    using AdInterFail = TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI.AdInterFail;
    using AdsRewardClick = TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI.AdsRewardClick;
    using AdsRewardFail = TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI.AdsRewardFail;
    using AdsRewardOffer = TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.ABI.AdsRewardOffer;
    using UnityEngine.Scripting;

    public class FalconAnalyticEventFactory : BaseAnalyticEventFactory
    {
        #region Inject

        private readonly IIapServices    iapServices;
        private readonly IScreenManager   screenManager;
        private readonly FalconLocalData falconLocalData;

        [Preserve]
        public FalconAnalyticEventFactory(SignalBus signalBus,
                                          IAnalyticServices analyticServices,
                                          IIapServices iapServices,
                                          IScreenManager screenManager,
                                          FalconLocalData falconLocalData)
            : base(signalBus, analyticServices)
        {
            this.iapServices     = iapServices;
            this.screenManager   = screenManager;
            this.falconLocalData = falconLocalData;

            signalBus.Subscribe<LevelEndedSignal>(this.OnLevelEnded);
            signalBus.Subscribe<LevelSkippedSignal>(this.OnLevelSkipped);
            signalBus.Subscribe<OnIAPPurchaseSuccessSignal>(this.OnPurchaseComplete);

            signalBus.Subscribe<InterstitialAdCalledSignal>(this.OnShowInterstitialAd);
            signalBus.Subscribe<RewardedAdCalledSignal>(this.OnShowRewardedAd);

            signalBus.Subscribe<OnUpdateCurrencySignal>(this.OnUpdateCurrency);
            signalBus.Subscribe<ScreenShowSignal>(this.OnScreenShow);
        }

        #endregion

        public override string LevelMaxProperty             => "level";
        public override string LastLevelProperty            => "last_level";
        public override string LastAdsPlacementProperty     => "last_placement";
        public override string TotalInterstitialAdsProperty => "total_interstitial_ads";
        public override string TotalRewardedAdsProperty     => "total_rewarded_ads";
        public override string RetentionDayProperty         => "retent_type";

        public override AnalyticsEventCustomizationConfig AppsFlyerAnalyticsEventCustomizationConfig { get; set; } = new()
        {
            IgnoreEvents = new HashSet<Type>()
            {
                typeof(GameStarted),
                typeof(AdInterClick),
                typeof(AdInterFail),
                typeof(AdInterDownloaded),
                typeof(AdsRewardFail),
                typeof(AdsRewardOffer),
                typeof(AdsRewardedDownloaded),
                typeof(AdsRewardClick),
                typeof(LevelComplete),
            },
            CustomEventKeys = new Dictionary<string, string>()
            {
                { nameof(CommonEvents.TutorialCompletion), "af_tutorial_completion" },
                { "success", "af_success" },
                { "tutorialId", "af_tutorial_id" },
                { nameof(LevelEnd), "af_level_achieved" },
                { "level", "af_level" },
                { nameof(InterstitialAdCalled), "af_inters_show" },
                { nameof(InterstitialAdDisplayed), "af_inters_displayed" },
                { nameof(RewardedAdCalled), "af_rewarded_show" },
                { nameof(RewardedAdDisplayed), "af_rewarded_displayed" },

                // TODO: add af_achievement_unlocked archivement unlock event
            }
        };

        public override AnalyticsEventCustomizationConfig FireBaseAnalyticsEventCustomizationConfig { get; set; } = new()
        {
            IgnoreEvents = new HashSet<Type>(),
            CustomEventKeys = new Dictionary<string, string>()
            {
                { nameof(AdsRevenueEvent), "ad_impression_falcon" },
                { "AdsRevenueSourceId", "ad_platform" },
                { "AdNetwork", "ad_source" },
                { "AdUnit", "ad_unit_name" },
                { "AdFormat", "ad_format" },
                { "Placement", "placement" },
                { "Currency", "currency" },
                { "Revenue", "value" },
                { nameof(LevelEnd), "level_complete" },
                { "timePlay", "timeplayed" },
                { nameof(CommonEvents.LevelStart), "level_start" },
                { "gold", "current_gold" },

                { nameof(RewardedAdLoaded), "ads_reward_load" },
                { nameof(RewardedAdLoadClicked), "ads_reward_click" },
                { nameof(RewardedAdDisplayed), "ads_reward_show_success" },
                { nameof(RewardedAdShowFail), "ads_reward_show_fail" },
                { nameof(RewardedAdCompleted), "ads_reward_complete" },
                { nameof(InterstitialAdLoadFailed), "ad_inter_load_fail" },
                { nameof(InterstitialAdDownloaded), "ad_inter_load_success" },
                { nameof(InterstitialAdDisplayed), "ad_inter_show" },
                { nameof(InterstitialAdClicked), "ad_inter_click" },
            }
        };

        public override IEvent LevelLose(int level, int timeSpent, int loseCount)
        {
            this.analyticServices.Track(new CustomEvent()
            {
                EventName = "level_fail",
                EventProperties = new Dictionary<string, object>()
                {
                    { "level", level },
                    { "time_spent", timeSpent },
                    { "failcount", loseCount },
                    { "timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds() }
                }
            });
            return new LevelEnd(level, "lose", 0, timeSpent, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        }

        public override IEvent LevelWin(int level, int timeSpent, int winCount) { return new LevelEnd(level, "win", 0, timeSpent, DateTimeOffset.UtcNow.ToUnixTimeSeconds()); }

        public override IEvent LevelSkipped(int level, int timeSpent) { return new LevelEnd(level, "skip", 0, timeSpent, DateTimeOffset.UtcNow.ToUnixTimeSeconds()); }

        #region Falcon SDK log

        private void OnLevelEnded(LevelEndedSignal obj)
        {
            LevelStatus levelStatus;
            if (obj.IsWin)
            {
                var levelNotPassBefore = this.falconLocalData.PassedLevels.Add(obj.Level);
                levelStatus = levelNotPassBefore ? LevelStatus.Pass : LevelStatus.ReplayPass;
                if (levelNotPassBefore)
                {
                    this.LogCheckpoint(obj.Level);
                }
            }
            else
            {
                levelStatus = this.falconLocalData.PassedLevels.Contains(obj.Level) ? LevelStatus.ReplayFail : LevelStatus.Fail;
            }

            new FLevelLog(obj.Level, "normal", levelStatus, TimeSpan.FromSeconds(obj.Time)).Send();
        }

        private void OnLevelSkipped(LevelSkippedSignal obj)
        {
            if (this.falconLocalData.PassedLevels.Add(obj.Level)) this.LogCheckpoint(obj.Level);

            new FLevelLog(obj.Level, "normal", LevelStatus.Skip, TimeSpan.FromSeconds(obj.Time)).Send();
        }

        private void LogCheckpoint(int level)
        {
            if (level > 20) return;
            this.analyticServices.Track(new CustomEvent()
            {
                EventName = $"checkpoint_{level}"
            });
        }

        private void OnScreenShow(ScreenShowSignal obj)
        {
            var currentScreen = this.screenManager.CurrentActiveScreen.Value;
            new FActionLog($"ShowScreen_{currentScreen.GetType().Name}").Send();
        }

        private void OnUpdateCurrency(OnUpdateCurrencySignal obj)
            => new FResourceLog(obj.Amount > 0 ? FlowType.Source : FlowType.Sink,
                "in_game",
                LimitStringLog(obj.Id, 20),
                LimitStringLog(obj.Id),
                Math.Abs(obj.Amount),
                this.MaxPassLevel).Send();

        private void OnShowRewardedAd(RewardedAdCalledSignal obj) { new FAdLog(AdType.Reward, this.CurrentScreen, this.MaxPassLevel).Send(); }

        private void OnShowInterstitialAd(InterstitialAdCalledSignal obj) { new FAdLog(AdType.Interstitial, this.CurrentScreen, this.MaxPassLevel).Send(); }

        private void OnPurchaseComplete(OnIAPPurchaseSuccessSignal obj)
        {
            //TODO: Implement transaction ID
            var productData   = this.iapServices.GetProductData(obj.Product.Id);
            var transactionID = "obj.PurchasedProduct.transactionID";
            var where         = this.CurrentScreen;
            new FInAppLog(obj.Product.Id, productData.Price, productData.CurrencyCode, where, transactionID, this.screenManager.CurrentActiveScreen.Value.ToString()).Send();
            throw new NotImplementedException();
        }

        private string CurrentScreen => LimitStringLog(this.screenManager.CurrentActiveScreen.Value.GetType().Name);
        private int    MaxPassLevel  => this.falconLocalData.PassedLevels.Count;

        private static string LimitStringLog(string value, int length = 50)
        {
            var result = value.Length > length ? value[..length] : value;
            return string.IsNullOrEmpty(result) ? "unknown" : result;
        }

        #endregion
    }
}
#endif