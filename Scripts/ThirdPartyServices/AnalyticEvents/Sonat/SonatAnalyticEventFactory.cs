#if SONAT
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Sonat
{
    using System.Collections.Generic;
    using Core.AnalyticServices;
    using Core.AnalyticServices.Data;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
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
            IScreenManager                screenManager,
            ILoggerManager                loggerManager
        ) : base(signalBus, analyticServices, levelDataController)
        {
            this.levelDataController = levelDataController;
            this.screenManager       = screenManager;
            this.logger              = loggerManager.GetLogger(this);
        }

        private IEvent EndLevel(int timeSpent, bool isSuccess, string loseCause, string flow, LevelData levelData)
        {
            var startCount  = levelData.WinCount + levelData.LoseCount;
            var isFirstPlay = levelData.WinCount == 0 && levelData.LoseCount == 0;

            this.IsInGame = false;

            return new EndLevel(this.levelDataController.CurrentMode, levelData.Level.ToString(), startCount, timeSpent, isFirstPlay, isSuccess, loseCause, flow);
        }

        private bool   IsInGame { get; set; }
        private string Location => this.IsInGame ? "in_game" : "out_game";

        // reference link: "https://docs.google.com/spreadsheets/d/1jkLaX4Q-lErmM9WhC2k9BSmpiXOET2ywu5FJfNtQ3oU/edit?gid=1260577405#rangeid=864661828"
        private object GetMetadataValue(Dictionary<string, object> metadata, string key)
        {
            if (metadata.TryGetValue(key, out var value))
            {
                return value;
            }

            this.logger.Error($"Could not find metadata for {key}");

            return null;
        }

        // mapping metadata event data by "screen" param, value should be screen name or screen type
        private string GetScreen(Dictionary<string, object> metadata) => this.GetMetadataValue(metadata, "screen").ToString();

        // mapping metadata event data by "item_type" param, value should be [virtual_currency_type] or "pack", "feature", "ads"
        private string GetItemType(Dictionary<string, object> metadata) => this.GetMetadataValue(metadata, "item_type").ToString();

        // mapping metadata event data by "item_id" param, value should be [virtual_currency_name] or [pack_id], [feature_id], "rwd_ads"
        private string GetItemId(Dictionary<string, object> metadata) => this.GetMetadataValue(metadata, "item_id").ToString();

        // mapping metadata event data by "source" param, value should be "iap" or "non_iap"
        private string GetSource(Dictionary<string, object> metadata) => this.GetMetadataValue(metadata, "source").ToString();
        
        public override IEvent LevelStart(int level, int gold, Dictionary<string, object> metadata = null)
        {
            var levelData   = this.levelDataController.GetLevelData(level);
            var startCount  = levelData.WinCount + levelData.LoseCount;
            var isFirstPlay = levelData.WinCount == 0 && levelData.LoseCount == 0;

            this.IsInGame = true;

            return new StartLevel(this.levelDataController.CurrentMode, levelData.Level.ToString(), startCount, isFirstPlay);
        }

        public override IEvent LevelLose(int level, int timeSpent, int loseCount, Dictionary<string, object> metadata = null)
        {
            return this.EndLevel(timeSpent, false, "lose", "", this.levelDataController.GetLevelData(level));
        }

        public override IEvent LevelWin(int level, int timeSpent, int winCount, Dictionary<string, object> metadata = null)
        {
            return this.EndLevel(timeSpent, true, "", "", this.levelDataController.GetLevelData(level));
        }

        public override IEvent LevelSkipped(int level, int timeSpent, Dictionary<string, object> metadata = null)
        {
            return this.EndLevel(timeSpent, false, "skipped", "", this.levelDataController.GetLevelData(level));
        }

        public override IEvent InterstitialShow(int level, string place, Dictionary<string, object> metadata = null)
        {
            return new InterstitialShow(this.Location, this.GetScreen(metadata), place, level.ToString(), this.levelDataController.CurrentMode);
        }

        public override IEvent RewardedVideoShow(int level, string place, Dictionary<string, object> metadata = null)
        {
            return new VideoRewarded(this.levelDataController.CurrentMode, level.ToString(), "", this.Location, place, this.GetItemType(metadata), this.GetItemId(metadata));
        }

        public override IEvent EarnVirtualCurrency(string virtualCurrencyName, long value, string placement, int level, Dictionary<string, object> metadata = null)
        {
            return new EarnVirtualCurrency(virtualCurrencyName, value, this.Location, this.GetScreen(metadata), this.GetSource(metadata), this.GetItemType(metadata), this.GetItemId(metadata));
        }

        public override IEvent SpendVirtualCurrency(string virtualCurrencyName, long value, string placement, int level, Dictionary<string, object> metadata = null)
        {
            return new SpendVirtualCurrency(virtualCurrencyName, value, this.Location, this.GetScreen(metadata), this.GetItemType(metadata), this.GetItemId(metadata));
        }

        public override IEvent TutorialCompletion(bool success, string tutorialId)
        {
            return new TutorialComplete(tutorialId, 1);
        }
    }
}
#endif