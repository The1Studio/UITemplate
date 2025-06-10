#if SONAT
namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Sonat
{
    using System.Collections.Generic;
    using Core.AnalyticServices;
    using Core.AnalyticServices.Data;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine.Scripting;

    public sealed class SonatAnalyticEventFactory : BaseAnalyticEventFactory
    {
        private readonly UITemplateLevelDataController levelDataController;
        private readonly IScreenManager                screenManager;

        [Preserve]
        public SonatAnalyticEventFactory
        (
            SignalBus                     signalBus,
            IAnalyticServices             analyticServices,
            UITemplateLevelDataController levelDataController,
            IScreenManager                screenManager
        ) : base(signalBus, analyticServices, levelDataController)
        {
            this.levelDataController = levelDataController;
            this.screenManager       = screenManager;
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
        private const string ScreenParam   = "screen";    // mapping metadata event data by "screen" param, value should be screen name or screen type
        private const string ItemTypeParam = "item_type"; // mapping metadata event data by "item_type" param, value should be [virtual_currency_type] or "pack", "feature", "ads"
        private const string ItemIdParam   = "item_id";   // mapping metadata event data by "item_id" param, value should be [virtual_currency_name] or [pack_id], [feature_id], "rwd_ads"
        private const string SourceIdParam = "source";    // mapping metadata event data by "source" param, value should be "iap" or "non_iap"

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
            return new InterstitialShow(this.Location, metadata[ScreenParam].ToString(), place, level.ToString(), this.levelDataController.CurrentMode);
        }

        public override IEvent RewardedVideoShow(int level, string place, Dictionary<string, object> metadata = null)
        {
            return new VideoRewarded(this.levelDataController.CurrentMode, level.ToString(), "", this.Location, place, metadata[ItemTypeParam].ToString(), metadata[ItemIdParam].ToString());
        }

        public override IEvent EarnVirtualCurrency(string virtualCurrencyName, long value, string placement, int level, Dictionary<string, object> metadata = null)
        {
            return new EarnVirtualCurrency(virtualCurrencyName, value, this.Location, metadata[ScreenParam].ToString(), metadata[ItemTypeParam].ToString(), metadata[ItemIdParam].ToString());
        }

        public override IEvent SpendVirtualCurrency(string virtualCurrencyName, long value, string placement, int level, Dictionary<string, object> metadata = null)
        {
            return new SpendVirtualCurrency(virtualCurrencyName, value, this.Location, metadata[ScreenParam].ToString(), metadata[ItemTypeParam].ToString(), metadata[ItemIdParam].ToString());
        }

        public override IEvent TutorialCompletion(bool success, string tutorialId)
        {
            return new TutorialComplete(tutorialId, 1);
        }
    }
}
#endif