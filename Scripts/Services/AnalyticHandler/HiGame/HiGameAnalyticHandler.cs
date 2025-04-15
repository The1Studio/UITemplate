namespace TheOneStudio.UITemplate.UITemplate.Services.AnalyticHandler.HiGame
{
#if HIGAME
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents;
    using UnityEngine.Scripting;

    public class HiGameAnalyticHandler : UITemplateAnalyticHandler
    {
        #region Inject

        private readonly HiGameLocalData hiGameLocalData;
        
        [Preserve]
        public HiGameAnalyticHandler(
            SignalBus                           signalBus,
            IAnalyticServices                   analyticServices,
            IAnalyticEventFactory               analyticEventFactory,
            UITemplateLevelDataController       uiTemplateLevelDataController,
            UITemplateInventoryDataController   uITemplateInventoryDataController,
            UITemplateDailyRewardController     uiTemplateDailyRewardController,
            UITemplateGameSessionDataController uITemplateGameSessionDataController,
            IScreenManager                      screenManager,
            UITemplateAdsController             uiTemplateAdsController,
            HiGameLocalData                     hiGameLocalData
        ) : base(signalBus, analyticServices, analyticEventFactory, uiTemplateLevelDataController, uITemplateInventoryDataController, uiTemplateDailyRewardController, uITemplateGameSessionDataController, screenManager, uiTemplateAdsController)
        {
            this.hiGameLocalData = hiGameLocalData;
        }

        #endregion

        public override void Initialize()
        {
            base.Initialize();

            this.signalBus.Subscribe<LevelEndedSignal>(this.OnLevelEnded);
            this.signalBus.Subscribe<LevelSkippedSignal>(this.OnLevelSkipped);
        }

        private void OnLevelEnded(LevelEndedSignal obj)
        {
            if (!obj.IsWin) return;
            var levelNotPassBefore = this.hiGameLocalData.PassedLevels.Add(obj.Level);
            if (levelNotPassBefore)
            {
                this.LogCheckpoint(obj.Level);
            }
        }

        private void OnLevelSkipped(LevelSkippedSignal obj)
        {
            if (this.hiGameLocalData.PassedLevels.Add(obj.Level)) this.LogCheckpoint(obj.Level);
        }

        private void LogCheckpoint(int level)
        {
            if (level > 20) return;
            this.analyticServices.Track(new CustomEvent()
            {
                EventName = $"checkpoint_{level}"
            });
        }
    }
#endif
}