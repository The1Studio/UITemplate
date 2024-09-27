namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using GameFoundation.DI;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Configs.GameEvents;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Popups;
    using TheOneStudio.UITemplate.UITemplate.Services.StoreRating;
    using UnityEngine;
    using UnityEngine.Scripting;

    public class UITemplateRateUsService : IInitializable
    {
        private readonly SignalBus                           signalBus;
        private readonly GameFeaturesSetting                 gameFeaturesSetting;
        private readonly UITemplateGameSessionDataController uiTemplateGameSessionDataController;
        private readonly IScreenManager                      screenManager;
        private readonly UITemplateStoreRatingHandler        storeRatingHandler;

        [Preserve]
        public UITemplateRateUsService(
            SignalBus                           signalBus,
            GameFeaturesSetting                 gameFeaturesSetting,
            UITemplateGameSessionDataController uiTemplateGameSessionDataController,
            IScreenManager                      screenManager,
            UITemplateStoreRatingHandler        storeRatingHandler
        )
        {
            this.signalBus                           = signalBus;
            this.gameFeaturesSetting                 = gameFeaturesSetting;
            this.uiTemplateGameSessionDataController = uiTemplateGameSessionDataController;
            this.screenManager                       = screenManager;
            this.storeRatingHandler                  = storeRatingHandler;
        }

        public void Initialize()
        {
            this.signalBus.Subscribe<ScreenShowSignal>(this.OnScreenShow);
            this.isShownInCurrentSession = false;
        }

        private async void OnScreenShow(ScreenShowSignal obj)
        {
            if (!this.IsScreenCanShowRateUs(obj.ScreenPresenter)) return;
            await this.screenManager.OpenScreen<UITemplateRateGamePopupPresenter>();
            this.isShownInCurrentSession = true;
        }

        private bool IsScreenCanShowRateUs(IScreenPresenter screenPresenter)
        {
            if (this.isShownInCurrentSession) return false;
            if (Time.realtimeSinceStartup < this.gameFeaturesSetting.RateUsConfig.DelayInSecondsTillShow) return false;
            if (!this.gameFeaturesSetting.RateUsConfig.isUsingCommonLogic) return false;
            if (this.storeRatingHandler.IsRated) return false;

            if (this.uiTemplateGameSessionDataController.OpenTime < this.gameFeaturesSetting.RateUsConfig.SessionToShow) return false;
            if (this.gameFeaturesSetting.RateUsConfig.isCustomScreenTrigger)
            {
                return this.gameFeaturesSetting.RateUsConfig.screenTriggerIds.Contains(screenPresenter.GetType().Name);
            }

            return true; // Show everywhere
        }

        private bool isShownInCurrentSession = false;
    }
}