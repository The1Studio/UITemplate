namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward;
    using Zenject;

    public class UITemplateDailyRewardService : IInitializable
    {
        #region inject

        private readonly ScreenManager                   screenManager;
        private readonly UITemplateUserDataController    uiTemplateUserDataController;
        private readonly UITemplateDailyRewardController uiTemplateDailyRewardController;

        #endregion

        public UITemplateDailyRewardService(ScreenManager screenManager, UITemplateUserDataController uiTemplateUserDataController, 
            UITemplateDailyRewardController uiTemplateDailyRewardController)
        {
            this.screenManager                   = screenManager;
            this.uiTemplateUserDataController    = uiTemplateUserDataController;
            this.uiTemplateDailyRewardController = uiTemplateDailyRewardController;
        }

        public async void ShowDailyRewardPopup(Action onClaimReward)
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
            
            await this.screenManager.OpenScreen<UITemplateDailyRewardPopupPresenter, UITemplateDailyRewardPopupModel>(new UITemplateDailyRewardPopupModel()
            {
                OnClaimFinish = onClaimReward
            });
        }

        public void Initialize() { }
    }
}