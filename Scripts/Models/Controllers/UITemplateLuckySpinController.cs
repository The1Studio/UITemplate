namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using Cysharp.Threading.Tasks;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using TheOneStudio.UITemplate.UITemplate.Services;

    public class UITemplateLuckySpinController
    {
        private readonly UITemplateLuckySpinData uiTemplateLuckySpinData;
        private readonly InternetService         internetService;

        public UITemplateLuckySpinController(UITemplateLuckySpinData uiTemplateLuckySpinData, InternetService internetService)
        {
            this.uiTemplateLuckySpinData = uiTemplateLuckySpinData;
            this.internetService         = internetService;
        }

        public async UniTask<bool> IsUsedFreeSpinToDay()
        {
            var isDiffDay = await this.internetService.IsDifferentDay(this.uiTemplateLuckySpinData.LastSpinTime);

            if (!isDiffDay) return this.uiTemplateLuckySpinData.IsUsedFreeSpin;
            this.uiTemplateLuckySpinData.IsUsedFreeSpin              = false;

            return this.uiTemplateLuckySpinData.IsUsedFreeSpin;
        }

        public async void SaveTimeSpinToDay()
        {
            this.uiTemplateLuckySpinData.IsFirstTimeOpenLuckySpin = true;

            if ( this.uiTemplateLuckySpinData.IsUsedFreeSpin)
            {
                return;
            }

            this.uiTemplateLuckySpinData.IsUsedFreeSpin              = true;
            this.uiTemplateLuckySpinData.LastSpinTime                = await this.internetService.GetCurrentTimeAsync();
        }
    }
}