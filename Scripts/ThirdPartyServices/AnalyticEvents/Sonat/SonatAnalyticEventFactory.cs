#if SONAT
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Sonat
{
    using System;
    using System.Collections.Generic;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using Core.AnalyticServices.Data;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using GameFoundation.Signals;
    using global::TheOne.Logging;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine.Scripting;

    public sealed class SonatAnalyticEventFactory : BaseAnalyticEventFactory
    {
        private readonly UITemplateLevelDataController levelDataController;
        private readonly IScreenManager                screenManager;
        private readonly ILogger                       logger;

        [Preserve]
        public SonatAnalyticEventFactory
        (
            SignalBus                     signalBus,
            IAnalyticServices             analyticServices,
            UITemplateLevelDataController levelDataController,
            ILoggerManager                loggerManager,
            IScreenManager                screenManager
        ) : base(signalBus, analyticServices, levelDataController)
        {
            this.levelDataController = levelDataController;
            this.screenManager       = screenManager;
            this.logger              = loggerManager.GetLogger(this);
            signalBus.Subscribe<ScreenShowSignal>(this.OnScreenShow);
        }

        private string lastScreenName;
        private string lastScreenClass;

        public override AnalyticsEventCustomizationConfig FireBaseAnalyticsEventCustomizationConfig { get; set; } = new()
        {
            IgnoreEvents = new HashSet<Type>(),
            CustomEventKeys = new ()
            {
                { nameof(AdsRevenueEvent), "paid_ad_impression" },
                { "AdsRevenueSourceId", "ad_platform" },
                { "AdNetwork", "ad_source" },
                { "AdUnit", "ad_unit_name" },
                { "AdFormat", "ad_format" },
                { "Placement", "ad_placement" },
                { "Currency", "currency" },
                { "Revenue", "value" },
            }
        };

        private void OnScreenShow(ScreenShowSignal obj)
        {
            this.lastScreenName = obj.ScreenPresenter.GetType().Name;
            this.lastScreenClass = obj.ScreenPresenter.GetType().Name;
            this.TrackScreenView(this.lastScreenName, this.lastScreenClass);
        }

        private void TrackScreenView(string screenName, string screenClass)
        {
            this.analyticServices.Track(new CustomEvent()
            {
                EventName = "screen_view",
                EventProperties = new()
                {
                    { "screen_name", screenName },
                    { "screen_class", screenClass },
                }
            });
        }

        private IEvent EndLevel(int timeSpent, bool isSuccess, string loseCause, string flow, LevelData levelData)
        {
            var startCount  = levelData.WinCount + levelData.LoseCount;
            var isFirstPlay = levelData.WinCount == 0 && levelData.LoseCount == 0;

            this.IsInGame = false;
            
            return new CustomEvent()
            {
                EventName = "level_end",
                EventProperties = new ()
                {
                    { "mode", this.levelDataController.CurrentMode },
                    { "level", levelData.Level.ToString() },
                    { "start_count", startCount },
                    { "play_time", timeSpent },
                    { "is_first_play", isFirstPlay },
                    { "success", isSuccess },
                    { "lose_cause", loseCause },
                    { "flow_i", flow },
                }
            };
        }

        private bool   IsInGame { get; set; }
        private string Location => this.IsInGame ? "in_game" : "out_game";

        // reference link: "https://docs.google.com/spreadsheets/d/1jkLaX4Q-lErmM9WhC2k9BSmpiXOET2ywu5FJfNtQ3oU/edit?gid=1260577405#rangeid=864661828"
        private object GetMetadataValue(Dictionary<string, object> metadata, string key)
        {
            if (metadata == null)
            {
                this.logger.Error("Metadata is null");

                return null;
            }

            if (metadata.TryGetValue(key, out var value))
            {
                return value;
            }

            this.logger.Error($"Could not find metadata for {key}");

            return null;
        }

        private string GetScreen()
        {
            return this.screenManager.CurrentActiveScreen.Value != null ? this.screenManager.CurrentActiveScreen.Value.ScreenId : "null";
        }
        
        // Get class name of the current screen, used for analytics
        private string GetScreenClass()
        {
            return this.screenManager.CurrentActiveScreen.Value != null ? this.screenManager.CurrentActiveScreen.Value.GetType().Name : "null";
        }

        // mapping metadata event data by "item_type" param, value should be [virtual_currency_type] or "pack", "feature", "ads"
        private string GetItemType(Dictionary<string, object> metadata) => this.GetMetadataValue(metadata, "item_type")?.ToString();

        // mapping metadata event data by "item_id" param, value should be [virtual_currency_name] or [pack_id], [feature_id], "rwd_ads"
        private string GetItemId(Dictionary<string, object> metadata) => this.GetMetadataValue(metadata, "item_id")?.ToString();

        // mapping metadata event data by "source" param, value should be "iap" or "non_iap"
        private string GetSource(Dictionary<string, object> metadata) => this.GetMetadataValue(metadata, "source")?.ToString();
        
        // mapping metadata event data by "flow_i" param, value should be "flow_i" or ""    
        private string GetFlow(Dictionary<string, object> metadata) => metadata != null && metadata.TryGetValue("flow_i", out var flow) ? flow.ToString() : "";
        
        public override IEvent LevelStart(int level, int gold, Dictionary<string, object> metadata = null)
        {
            var levelData   = this.levelDataController.GetLevelData(level);
            var startCount  = levelData.WinCount + levelData.LoseCount + 1;
            var isFirstPlay = levelData.WinCount == 0 && levelData.LoseCount == 0;

            this.IsInGame = true;
           
            return new CustomEvent()
            {
                EventName = "level_start",
                EventProperties = new ()
                {
                    { "mode", this.levelDataController.CurrentMode },
                    { "level", levelData.Level.ToString() },
                    { "start_count", startCount },
                    { "is_first_play", isFirstPlay },
                }
            };
        }

        public override IEvent LevelLose(int level, int timeSpent, int loseCount, Dictionary<string, object> metadata = null)
        {
            return this.EndLevel(timeSpent, false, "lose", this.GetFlow(metadata), this.levelDataController.GetLevelData(level));
        }

        public override IEvent LevelWin(int level, int timeSpent, int winCount, Dictionary<string, object> metadata = null)
        {
            return this.EndLevel(timeSpent, true, "", this.GetFlow(metadata), this.levelDataController.GetLevelData(level));
        }

        public override IEvent LevelSkipped(int level, int timeSpent, Dictionary<string, object> metadata = null)
        {
            return this.EndLevel(timeSpent, false, "skipped", this.GetFlow(metadata), this.levelDataController.GetLevelData(level));
        }

        public override IEvent FTUEStart(string ftueId, Dictionary<string, object> metadata)
        {
            return new CustomEvent()
            {
                EventName = "tutorial_begin",
                EventProperties = new ()
                {
                    { "placement", ftueId },
                    { "step", metadata.TryGetValue("step", out var step) ? step.ToString() : null },
                }
            };
        }

        public override IEvent FTUECompleted(string completedId, Dictionary<string, object> metadata)
        {
            return new CustomEvent()
            {
                EventName = "tutorial_complete",        
                EventProperties = new ()
                {
                    { "placement", completedId },
                    { "step", metadata.TryGetValue("step", out var step) ? step.ToString() : null },
                }
            };
        }

        public override IEvent InterstitialShow(int level, string place, Dictionary<string, object> metadata = null)
        {
            this.TrackScreenView("IntersAds","IntersAds");
            return new CustomEvent()
            {
                EventName = "show_interstitial",
                EventProperties = new ()
                {
                    { "location", this.Location },
                    { "screen", this.GetScreen() },
                    { "placement", place },
                    { "level", level.ToString() },
                    { "mode", this.levelDataController.CurrentMode },
                }
            };
        }

        public override IEvent InterstitialShowCompleted(int level, string place)
        {
            this.TrackScreenView(this.lastScreenName,this.lastScreenClass);
            return base.InterstitialShowCompleted(level, place);
        }

        public override IEvent RewardedVideoShow(int level, string place, Dictionary<string, object> metadata = null)
        {
            this.TrackScreenView("RewardedAds","RewardedAds");
            return new CustomEvent()
            {
                EventName = "video_rewarded",
                EventProperties = new ()
                {
                    { "mode", this.levelDataController.CurrentMode },
                    { "level", level.ToString() },
                    { "phase", "1" },
                    { "location", this.Location },
                    { "placement", place },
                    { "item_type", this.GetItemType(metadata) },
                    { "item_id", this.GetItemId(metadata) },
                }
            };
        }

        public override IEvent RewardedVideoShowCompleted(int level, string place, bool isRewarded)
        {
            this.TrackScreenView(this.lastScreenName,this.lastScreenClass);
            return base.RewardedVideoShowCompleted(level, place, isRewarded);
        }

        public override IEvent EarnVirtualCurrency(string virtualCurrencyName, long value, string placement, int level, Dictionary<string, object> metadata = null)
        {
            return new CustomEvent()
            {
                EventName = "earn_virtual_currency",
                EventProperties = new ()
                {
                    { "virtual_currency_name", virtualCurrencyName },
                    { "virtual_currency_type", virtualCurrencyName },
                    { "value", value },
                    { "location", this.Location },
                    { "screen", this.GetScreen() },
                    { "source", this.GetSource(metadata) },
                    { "item_type", this.GetItemType(metadata) },
                    { "item_id", this.GetItemId(metadata) },
                }
            };
        }

        public override IEvent SpendVirtualCurrency(string virtualCurrencyName, long value, string placement, int level, Dictionary<string, object> metadata = null)
        {
            return new CustomEvent()
            {
                EventName = "spend_virtual_currency",
                EventProperties = new ()
                {
                    { "virtual_currency_name", virtualCurrencyName },
                    { "virtual_currency_type", virtualCurrencyName },
                    { "value", value },
                    { "location", this.Location },
                    { "screen", this.GetScreen() },
                    { "item_type", this.GetItemType(metadata) },
                    { "item_id", this.GetItemId(metadata) },
                }
            };
        }
        
        public override IEvent ClickWidgetGame(string widgetId, Dictionary<string, object> metadata = null)
        {
            return new CustomEvent()
            {
                EventName = "click_icon_shortcut",
                EventProperties = new ()
                {
                    { "shortcut", widgetId },
                    { "mode", this.Location },
                    { "level", this.GetScreen() },
                    { "placement", this.GetItemType(metadata) }
                }
            };
        }
        
        public override IEvent ShowPopupUI(string popupId, bool isAuto)
        {
            return new CustomEvent()
            {
                EventName = "show_ui",
                EventProperties = new ()
                {
                    { "ui_name", popupId },
                    { "ui_type", "popup" },
                    { "ui_class", this.GetScreenClass() },
                    { "open_by",  isAuto ? "auto" : "user" },
                    { "screen", this.GetScreen() },
                    { "placement", this.Location },
                    { "mode", this.levelDataController.CurrentMode },
                    { "level", this.levelDataController.CurrentLevel },
                }
            };
        }

    }
}
#endif