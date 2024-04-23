namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using TheOneStudio.UITemplate.UITemplate.Configs.GameEvents;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Popups;
    using TheOneStudio.UITemplate.UITemplate.Services.StoreRating;
    using Zenject;

    public class UITemplateRateUsService : IInitializable
    {
        private readonly SignalBus                           signalBus;
        private readonly GameFeaturesSetting                 gameFeaturesSetting;
        private readonly UITemplateGameSessionDataController uiTemplateGameSessionDataController;
        private readonly IScreenManager                      screenManager;
        private readonly UITemplateStoreRatingHandler        storeRatingHandler;

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

        public void Initialize() { this.signalBus.Subscribe<ScreenShowSignal>(this.OnScreenShow); }

        private async void OnScreenShow(ScreenShowSignal obj)
        {
            if (this.IsScreenCanShowRateUs(obj.ScreenPresenter))
            {
                await this.screenManager.OpenScreen<UITemplateRateGameScreenPresenter>();
            }
        }

        private bool IsScreenCanShowRateUs(IScreenPresenter screenPresenter)
        {
            if (this.storeRatingHandler.IsRated) return false;
            if (this.uiTemplateGameSessionDataController.OpenTime < this.gameFeaturesSetting.RateUsConfig.SessionToShow) return false;
            if (this.gameFeaturesSetting.DailyRewardConfig.isCustomScreenTrigger)
            {
                return this.gameFeaturesSetting.RateUsConfig.screenTriggerIds.Contains(screenPresenter.GetType().Name);
            }

            return screenPresenter is UITemplateHomeSimpleScreenPresenter or UITemplateHomeTapToPlayScreenPresenter;
        }
    }
}