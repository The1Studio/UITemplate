#if ATHENA
namespace TheOneStudio.UITemplate.UITemplate.Services.AnalyticHandler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.AnalyticServices;
    using Core.AnalyticServices.CommonEvents;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.Utilities.ApplicationServices;
    using GameFoundation.Signals;
    using ServiceImplementation.IAPServices.Signals;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Apero;
    using UnityEngine.Scripting;

    public class AthenaAnalyticHandler : UITemplateAnalyticHandler
    {
        #region Inject

        private readonly UITemplateLevelDataController levelDataController;
        private readonly UITemplateShopPackBlueprint   shopPackBlueprint;

        [Preserve]
        public AthenaAnalyticHandler(
            SignalBus signalBus,
            IAnalyticServices analyticServices,
            IAnalyticEventFactory analyticEventFactory,
            UITemplateLevelDataController uiTemplateLevelDataController,
            UITemplateInventoryDataController uITemplateInventoryDataController,
            UITemplateDailyRewardController uiTemplateDailyRewardController,
            UITemplateGameSessionDataController uITemplateGameSessionDataController,
            IScreenManager screenManager,
            UITemplateLevelDataController levelDataController,
            UITemplateAdsController adsController,
            UITemplateShopPackBlueprint shopPackBlueprint) : base(signalBus, analyticServices, analyticEventFactory, uiTemplateLevelDataController,
            uITemplateInventoryDataController, uiTemplateDailyRewardController,
            uITemplateGameSessionDataController, screenManager, adsController)
        {
            this.levelDataController = levelDataController;
            this.shopPackBlueprint   = shopPackBlueprint;
        }

        #endregion

        private bool   isLevelAbandoned;
        private string uniqueId;
        public override void Initialize()
        {
            base.Initialize();
            this.signalBus.Subscribe<OnIAPPurchaseSuccessSignal>(this.OnIAPPurchaseSuccessHandler);
            this.signalBus.Subscribe<LevelStartedSignal>(this.OnLevelStartHandler);
            this.signalBus.Subscribe<LevelEndedSignal>(this.OnLevelEndedHandler);
            this.signalBus.Subscribe<OnUpdateCurrencySignal>(this.OnUpdateCurrencyHandler);
            this.signalBus.Subscribe<ApplicationQuitSignal>(this.OnApplicationQuitHandler);
        }

        private void OnIAPPurchaseSuccessHandler(OnIAPPurchaseSuccessSignal signal)
        {
            this.analyticServices.Track(new Purchase(signal.Product));
            this.analyticServices.Track(new CustomEvent
            {
                EventName = "bi_business_event",
                EventProperties = new()
                {
                    { "product_name", this.shopPackBlueprint[signal.Product.Id].Name },
                    { "product_id", signal.Product.Id },
                    { "quantity", signal.Quantity },
                    { "price", signal.Product.Price },
                    { "currency", signal.Product.CurrencyCode },
                    { "transaction_id", string.Empty },
                },
            });
        }

        private void OnLevelStartHandler(LevelStartedSignal signal)
        {
            var level    = signal.Level;
            var leveData = this.levelDataController.GetLevelData(level);
            this.uniqueId = Guid.NewGuid().ToString();
            this.analyticServices.Track(new CustomEvent
            {
                EventName = "game_start",
                EventProperties = new()
                {
                    { "game_mode", signal.Mode },
                    { "ID", this.uniqueId },
                    { "level_id", level.ToString() },
                    { "attempts", leveData.LoseCount + leveData.WinCount + 1 }
                },
            });
        }

        private void OnLevelEndedHandler(LevelEndedSignal signal)
        {
            var level    = signal.Level;
            var leveData = this.levelDataController.GetLevelData(level);
            this.analyticServices.Track(new CustomEvent
            {
                EventName = "game_over",
                EventProperties = new()
                {
                    { "game_mode", signal.Mode },
                    { "ID", this.uniqueId },
                    { "level_id", level.ToString() },
                    { "time_spent", signal.Time },
                    { "context", signal.IsWin ? 1 : 0 },
                    { "level_abandoned", this.isLevelAbandoned ? 1 : 0 },
                    { "attempts", leveData.LoseCount + leveData.WinCount }
                },
            });
        }

        private void OnUpdateCurrencyHandler(OnUpdateCurrencySignal signal)
        {
            var eventProperties = new Dictionary<string, object>()
            {
                { "flow_type", signal.Amount > 0 ? "source" : "sink" },
                { "virtual_currency_name", signal.Id },
                { "value", signal.Amount },
                { "level_id", this.levelDataController.CurrentLevel.ToString() },
                { "balance", signal.FinalValue }
            };
            if (signal.Amount >= 0)
            {
                eventProperties.Add("from", signal.Source);
            }
            else
            {
                eventProperties.Add("to", signal.Source);
            }

            if (signal.Metadata != null)
            {
                foreach (var (key, value) in signal.Metadata)
                {
                    eventProperties.TryAdd(key, value);
                }
            }

            this.analyticServices.Track(new CustomEvent
            {
                EventName       = "bi_resource_event",
                EventProperties = eventProperties,
            });
        }

        private void OnApplicationQuitHandler()
        {
            this.isLevelAbandoned = true;
            this.levelDataController.LoseCurrentLevel();
        }
    }
}

#endif