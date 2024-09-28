namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.DI;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using GameFoundation.Scripts.UIModule.Utilities.GameQueueAction;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Configs.GameEvents;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.FeaturesConfig;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward;
    using UnityEngine.Scripting;

    public class UITemplateDailyRewardService : IInitializable
    {
        private const string FirstOpenAppKey = "FirstOpenApp";

        #region inject

        private readonly SignalBus                           signalBus;
        private readonly UITemplateDailyRewardController     uiTemplateDailyRewardController;
        private readonly INotificationService                notificationServices;
        private readonly GameQueueActionContext              gameQueueActionContext;
        private readonly UITemplateFeatureConfig             uiTemplateFeatureConfig;
        private readonly UITemplateGameSessionDataController sessionDataController;
        private readonly GameFeaturesSetting                 gameFeaturesSetting;

        #endregion

        private bool canShowReward = true;

        [Preserve]
        public UITemplateDailyRewardService(
            SignalBus                           signalBus,
            UITemplateDailyRewardController     uiTemplateDailyRewardController,
            INotificationService                notificationServices,
            GameQueueActionContext              gameQueueActionContext,
            UITemplateFeatureConfig             uiTemplateFeatureConfig,
            UITemplateGameSessionDataController sessionDataController,
            GameFeaturesSetting                 gameFeaturesSetting
        )
        {
            this.signalBus                       = signalBus;
            this.uiTemplateDailyRewardController = uiTemplateDailyRewardController;
            this.notificationServices            = notificationServices;
            this.gameQueueActionContext          = gameQueueActionContext;
            this.uiTemplateFeatureConfig         = uiTemplateFeatureConfig;
            this.sessionDataController           = sessionDataController;
            this.gameFeaturesSetting             = gameFeaturesSetting;
        }

        public void Initialize() { this.signalBus.Subscribe<ScreenShowSignal>(this.OnScreenShow); }

        private bool IsFirstOpenGame() { return this.sessionDataController.OpenTime == 1; }

        public UniTask ShowDailyRewardPopupAsync(bool force = false)
        {
            this.ShowDailyRewardPopup(force);
            return UniTask.CompletedTask;
        }

        private void ShowDailyRewardPopup(bool force)
        {
            if (!force)
            {
                if (!this.canShowReward)
                    return;

                if (!this.gameFeaturesSetting.DailyRewardConfig.showOnFirstOpen &&
                    this.IsFirstOpenGame())
                {
                    this.canShowReward = false;
                    return;
                }

                if (!this.uiTemplateDailyRewardController.CanClaimReward)
                    return;
            }

            this.notificationServices.SetupCustomNotification(this.gameFeaturesSetting.DailyRewardConfig.notificationId);
            this.gameQueueActionContext.AddScreenToQueueAction<UITemplateDailyRewardPopupPresenter, UITemplateDailyRewardPopupModel>(new UITemplateDailyRewardPopupModel());
        }

        private async void OnScreenShow(ScreenShowSignal obj)
        {
            if (this.IsScreenCanShowDailyReward(obj.ScreenPresenter))
            {
                await this.uiTemplateDailyRewardController.CheckRewardStatus();
                if (!this.uiTemplateFeatureConfig.IsDailyRewardEnable) return;
                await this.ShowDailyRewardPopupAsync();
            }
        }

        private bool IsScreenCanShowDailyReward(IScreenPresenter screenPresenter)
        {
            if (this.gameFeaturesSetting.DailyRewardConfig.isCustomScreenTrigger)
            {
                return this.gameFeaturesSetting.DailyRewardConfig.screenTriggerIds.Contains(screenPresenter.GetType().Name);
            }

            return screenPresenter is UITemplateHomeSimpleScreenPresenter or UITemplateHomeTapToPlayScreenPresenter;
        }
    }
}