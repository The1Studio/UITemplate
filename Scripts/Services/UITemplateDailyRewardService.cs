namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using GameFoundation.Scripts.UIModule.Utilities.GameQueueAction;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward;
    using UnityEngine;
    using Zenject;

    public class UITemplateDailyRewardService : IInitializable
    {
        private const string FirstOpenAppKey = "FirstOpenApp";
        private const string NotificationId  = "daily_reward";


        #region inject

        private readonly SignalBus                       signalBus;
        private readonly ScreenManager                   screenManager;
        private readonly UITemplateDailyRewardController uiTemplateDailyRewardController;
        private readonly NotificationServices            notificationServices;
        private readonly GameQueueActionContext          gameQueueActionContext;

        #endregion

        private bool canShowReward = true;

        public UITemplateDailyRewardService(SignalBus            signalBus,            ScreenManager          screenManager, UITemplateDailyRewardController uiTemplateDailyRewardController,
                                            NotificationServices notificationServices, GameQueueActionContext gameQueueActionContext)
        {
            this.signalBus                       = signalBus;
            this.screenManager                   = screenManager;
            this.uiTemplateDailyRewardController = uiTemplateDailyRewardController;
            this.notificationServices            = notificationServices;
            this.gameQueueActionContext          = gameQueueActionContext;
        }

        public void Initialize()
        {
            this.signalBus.Subscribe<ScreenShowSignal>(this.OnScreenShow);
        }

        private bool IsFirstOpenGame()
        {
            if (PlayerPrefs.GetInt(FirstOpenAppKey) != 0) return false;
            PlayerPrefs.SetInt(FirstOpenAppKey, 1);
            return true;
        }

        public async UniTask ShowDailyRewardPopupAsync(bool force = false)
        {
            await this.uiTemplateDailyRewardController.CheckRewardStatus();
            this.ShowDailyRewardPopup(this.OnClaimDailyRewardFinish, force);
        }

        private void ShowDailyRewardPopup(Action onClaimReward, bool force)
        {
            if (!this.canShowReward && !force)
                return;

            if (this.IsFirstOpenGame())
            {
                onClaimReward?.Invoke();
                this.canShowReward = false;
                this.notificationServices.SetupCustomNotification(NotificationId);
                return;
            }

            if (!this.uiTemplateDailyRewardController.CanClaimReward && !force)
                return;

            this.gameQueueActionContext.AddScreenToQueueAction<UITemplateDailyRewardPopupPresenter, UITemplateDailyRewardPopupModel>(new UITemplateDailyRewardPopupModel()
            {
                OnClaimFinish = onClaimReward
            });
        }

        private async void OnScreenShow(ScreenShowSignal obj)
        {
            if (obj.ScreenPresenter is UITemplateHomeSimpleScreenPresenter or UITemplateHomeTapToPlayScreenPresenter)
            {
                await this.ShowDailyRewardPopupAsync();
            }
        }

        private void OnClaimDailyRewardFinish()
        {
        }
    }
}