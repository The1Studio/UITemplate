namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward;
    using Zenject;

    public class UITemplateDailyRewardService : IInitializable
    {
        #region inject

        private readonly SignalBus                       signalBus;
        private readonly ScreenManager                   screenManager;
        private readonly UITemplateUserDataController    uiTemplateUserDataController;
        private readonly UITemplateDailyRewardController uiTemplateDailyRewardController;

        #endregion

        public UITemplateDailyRewardService(SignalBus signalBus, ScreenManager screenManager, UITemplateUserDataController uiTemplateUserDataController,
            UITemplateDailyRewardController uiTemplateDailyRewardController)
        {
            this.signalBus                       = signalBus;
            this.screenManager                   = screenManager;
            this.uiTemplateUserDataController    = uiTemplateUserDataController;
            this.uiTemplateDailyRewardController = uiTemplateDailyRewardController;
        }

        private void ShowDailyRewardPopup(Action onClaimReward)
        {
            if (this.uiTemplateUserDataController.IsFirstOpenGame)
            {
                this.uiTemplateUserDataController.SetIsFirstOpenGame(false);
                onClaimReward?.Invoke();
                this.uiTemplateDailyRewardController.ResetRewardStatus();

                return;
            }

            if (!this.uiTemplateDailyRewardController.CanClaimReward)
                return;

            _ = this.screenManager.OpenScreen<UITemplateDailyRewardPopupPresenter, UITemplateDailyRewardPopupModel>(new UITemplateDailyRewardPopupModel()
            {
                OnClaimFinish = onClaimReward
            });
        }

        public void Initialize() { this.signalBus.Subscribe<ScreenShowSignal>(this.OnScreenShow); }

        private void OnScreenShow(ScreenShowSignal obj)
        {
            if (obj.ScreenPresenter is UITemplateHomeSimpleScreenPresenter or UITemplateHomeTapToPlayScreenPresenter)
                this.ShowDailyRewardPopup(this.OnClaimDailyRewardFinish);
        }

        private void OnClaimDailyRewardFinish() { }
    }
}