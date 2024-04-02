namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using TheOneStudio.UITemplate.UITemplate.Configs.GameEvents;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Popups;
    using Zenject;

    public class UITemplateNoInternetService : IInitializable
    {
        private readonly SignalBus                           signalBus;
        private readonly GameFeaturesSetting                 gameFeaturesSetting;
        private readonly IScreenManager                      screenManager;
        private readonly UITemplateGameSessionDataController uiTemplateGameSessionDataController;

        public UITemplateNoInternetService(
            SignalBus signalBus,
            GameFeaturesSetting gameFeaturesSetting,
            IScreenManager screenManager,
            UITemplateGameSessionDataController uiTemplateGameSessionDataController)
        {
            this.signalBus                           = signalBus;
            this.gameFeaturesSetting                 = gameFeaturesSetting;
            this.screenManager                       = screenManager;
            this.uiTemplateGameSessionDataController = uiTemplateGameSessionDataController;
        }

        public void Initialize() { this.signalBus.Subscribe<ScreenShowSignal>(this.OnScreenShow); }

        private async void OnScreenShow(ScreenShowSignal obj)
        {
            if (this.IsScreenCanShowDailyReward(obj.ScreenPresenter))
            {
                await this.screenManager.OpenScreen<UITemplateConnectErrorPresenter>();
            }
        }

        private bool IsScreenCanShowDailyReward(IScreenPresenter screenPresenter)
        {
            if (this.uiTemplateGameSessionDataController.OpenTime < this.gameFeaturesSetting.NoInternetConfig.SessionToShow) return false;
            if (this.gameFeaturesSetting.DailyRewardConfig.isCustomScreenTrigger)
            {
                return this.gameFeaturesSetting.NoInternetConfig.screenTriggerIds.Contains(screenPresenter.GetType().Name);
            }

            return screenPresenter is UITemplateHomeSimpleScreenPresenter or UITemplateHomeTapToPlayScreenPresenter;
        }
    }
}