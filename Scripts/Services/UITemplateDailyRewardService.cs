namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using GameFoundation.Scripts.UIModule.Utilities.GameQueueAction;
    using TheOneStudio.UITemplate.UITemplate.Configs.GameEvents;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.FeaturesConfig;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward;
    using UnityEngine;
    using Zenject;

    public class UITemplateDailyRewardService : IInitializable
    {
        private const string FirstOpenAppKey = "FirstOpenApp";

        #region inject

        private readonly SignalBus                       signalBus;
        private readonly ScreenManager                   screenManager;
        private readonly UITemplateDailyRewardController uiTemplateDailyRewardController;
        private readonly INotificationService            notificationServices;
        private readonly GameQueueActionContext          gameQueueActionContext;
        private readonly UITemplateFeatureConfig         uiTemplateFeatureConfig;
        private readonly GameFeaturesSetting             gameFeaturesSetting;

        #endregion

        private bool canShowReward = true;

        public UITemplateDailyRewardService(SignalBus signalBus, ScreenManager screenManager, UITemplateDailyRewardController uiTemplateDailyRewardController,
            INotificationService notificationServices, GameQueueActionContext gameQueueActionContext, UITemplateFeatureConfig uiTemplateFeatureConfig,
            GameFeaturesSetting gameFeaturesSetting)
        {
            this.signalBus                       = signalBus;
            this.screenManager                   = screenManager;
            this.uiTemplateDailyRewardController = uiTemplateDailyRewardController;
            this.notificationServices            = notificationServices;
            this.gameQueueActionContext          = gameQueueActionContext;
            this.uiTemplateFeatureConfig         = uiTemplateFeatureConfig;
            this.gameFeaturesSetting             = gameFeaturesSetting;
        }

        public void Initialize() { this.signalBus.Subscribe<ScreenShowSignal>(this.OnScreenShow); }

        private bool IsFirstOpenGame()
        {
            if (PlayerPrefs.GetInt(FirstOpenAppKey) != 0) return false;
            PlayerPrefs.SetInt(FirstOpenAppKey, 1);
            return true;
        }

        public UniTask ShowDailyRewardPopupAsync(bool force = false)
        {
            this.ShowDailyRewardPopup(this.OnClaimDailyRewardFinish, force);
            return UniTask.CompletedTask;
        }

        private void ShowDailyRewardPopup(Action onClaimReward, bool force)
        {
            if (!this.canShowReward && !force)
                return;

            if (!this.gameFeaturesSetting.DailyRewardConfig.showOnFirstOpen && this.IsFirstOpenGame())
            {
                onClaimReward?.Invoke();
                this.canShowReward = false;
                return;
            }

            if (!this.uiTemplateDailyRewardController.CanClaimReward && !force)
                return;

            this.notificationServices.SetupCustomNotification(this.gameFeaturesSetting.DailyRewardConfig.notificationId);
            this.gameQueueActionContext.AddScreenToQueueAction<UITemplateDailyRewardPopupPresenter, UITemplateDailyRewardPopupModel>(new UITemplateDailyRewardPopupModel()
            {
                OnClaimFinish       = onClaimReward,
                IsGetNextDayWithAds = this.gameFeaturesSetting.DailyRewardConfig.getNextDayWithAds
            });
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
                return this.gameFeaturesSetting.DailyRewardConfig.screenTriggerIds.Contains(screenPresenter.ScreenId);
            }

            return screenPresenter is UITemplateHomeSimpleScreenPresenter or UITemplateHomeTapToPlayScreenPresenter;
        }

        private void OnClaimDailyRewardFinish() { }
    }
}