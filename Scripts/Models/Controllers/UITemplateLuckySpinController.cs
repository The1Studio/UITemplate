namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using System;
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

        public bool IsUsedFreeSpinToDay()
        {
            var isDiffDay = this.internetService.IsDifferentDay(this.uiTemplateLuckySpinData.LastSpinTime, DateTime.Now);

            if (!isDiffDay) return this.uiTemplateLuckySpinData.IsUsedFreeSpin;
            this.uiTemplateLuckySpinData.IsUsedFreeSpin = false;

            return this.uiTemplateLuckySpinData.IsUsedFreeSpin;
        }

        public void SaveTimeSpinToDay()
        {
            this.uiTemplateLuckySpinData.IsFirstTimeOpenLuckySpin = true;

            if (this.uiTemplateLuckySpinData.IsUsedFreeSpin)
            {
                return;
            }

            this.uiTemplateLuckySpinData.IsUsedFreeSpin = true;
            this.uiTemplateLuckySpinData.LastSpinTime   = DateTime.Now;
        }
    }
}