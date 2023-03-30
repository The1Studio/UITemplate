namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using EasyMobile;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward;
    using UnityEngine;
    using Zenject;

    public class UITemplateDailyRewardService : IInitializable
    {
        #region inject

        private readonly SignalBus                       signalBus;
        private readonly ScreenManager                   screenManager;
        private readonly UITemplateDailyRewardController uiTemplateDailyRewardController;
        private readonly NotificationServices            notificationServices;

        #endregion

        private bool canShowReward = true;

        public UITemplateDailyRewardService(SignalBus signalBus, ScreenManager screenManager,
            UITemplateDailyRewardController uiTemplateDailyRewardController, NotificationServices notificationServices)
        {
            this.signalBus                       = signalBus;
            this.screenManager                   = screenManager;
            this.uiTemplateDailyRewardController = uiTemplateDailyRewardController;
            this.notificationServices            = notificationServices;
        }

        public void Initialize()
        {
            this.signalBus.Subscribe<ScreenShowSignal>(this.OnScreenShow);
        }
        
        private bool IsFirstOpenGame()
        {
            const string firstOpenAppKey = "FirstOpenApp";
            if (PlayerPrefs.GetInt(firstOpenAppKey) != 0) return false;
            PlayerPrefs.SetInt(firstOpenAppKey, 1);
            return true;
        }

        private void ShowDailyRewardPopup(Action onClaimReward)
        {
            if (!this.canShowReward)
                return;
            
            if (this.IsFirstOpenGame())
            {
                onClaimReward?.Invoke();
                this.uiTemplateDailyRewardController.ResetRewardStatus();
                this.canShowReward = false;
                return;
            }

            if (!this.uiTemplateDailyRewardController.CanClaimReward)
                return;

            _ = this.screenManager.OpenScreen<UITemplateDailyRewardPopupPresenter, UITemplateDailyRewardPopupModel>(new UITemplateDailyRewardPopupModel()
            {
                OnClaimFinish = onClaimReward
            });
        }

        private void OnScreenShow(ScreenShowSignal obj)
        {
            if (obj.ScreenPresenter is UITemplateHomeSimpleScreenPresenter or UITemplateHomeTapToPlayScreenPresenter)
            {
                this.ShowDailyRewardPopup(this.OnClaimDailyRewardFinish);
            }
        }

        private void OnClaimDailyRewardFinish() { }
    }
}