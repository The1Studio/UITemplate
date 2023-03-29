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
        private readonly UITemplateDailyRewardController uiTemplateDailyRewardController;

        #endregion

        private bool canShowReward = true;

        public UITemplateDailyRewardService(SignalBus signalBus, ScreenManager screenManager,
            UITemplateDailyRewardController uiTemplateDailyRewardController)
        {
            this.signalBus                       = signalBus;
            this.screenManager                   = screenManager;
            this.uiTemplateDailyRewardController = uiTemplateDailyRewardController;
        }

        public void Initialize() { this.signalBus.Subscribe<ScreenShowSignal>(this.OnScreenShow); }

        private void ShowDailyRewardPopup(Action onClaimReward)
        {
            if (!this.canShowReward)
                return;
            
            if (this.uiTemplateDailyRewardController.IsFirstOpenGame())
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
                this.ShowDailyRewardPopup(this.OnClaimDailyRewardFinish);
        }

        private void OnClaimDailyRewardFinish() { }
    }
}