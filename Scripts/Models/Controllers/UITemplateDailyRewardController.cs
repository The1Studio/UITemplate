namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using Cysharp.Threading.Tasks;
    using TheOneStudio.UITemplate.UITemplate.Services;

    public class UITemplateDailyRewardController
    {
        #region inject

        private readonly IInternetService              internetService;
        private readonly UITemplateDailyRewardData uiTemplateDailyRewardData;

        #endregion

        public UITemplateDailyRewardController(IInternetService internetService, UITemplateDailyRewardData uiTemplateDailyRewardData)
        {
            this.internetService               = internetService;
            this.uiTemplateDailyRewardData = uiTemplateDailyRewardData;
        }

        public async UniTask<int> GetUserLoginDay() => (await this.internetService.GetCurrentTimeAsync()).Day - this.uiTemplateDailyRewardData.BeginDate.Day + 1;
    }
}