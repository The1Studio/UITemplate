namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward;
    using Zenject;

    public class UITemplateDailyRewardService : IInitializable
    {
        #region inject

        private readonly ScreenManager                       screenManager;

        #endregion

        public UITemplateDailyRewardService(ScreenManager screenManager)
        {
            this.screenManager             = screenManager;
        }

        public async void ShowDailyRewardPopup(Action onClaimReward)
        {
            await this.screenManager.OpenScreen<UITemplateDailyRewardPopupPresenter, UITemplateDailyRewardPopupModel>(new UITemplateDailyRewardPopupModel()
            {
                OnClaimFinish = onClaimReward
            });
        }
        
        public void Initialize()
        {
            
        }
    }
}