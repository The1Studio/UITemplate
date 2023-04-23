namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using System;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using UIModule.Utilities;

    public class UITemplateLuckySpinController:IUITemplateControllerData
    {
        private readonly UITemplateLuckySpinData           uiTemplateLuckySpinData;
        private readonly InternetService                   internetService;
        private readonly UITemplateInventoryDataController uiTemplateInventoryDataController;

        public UITemplateLuckySpinController(UITemplateLuckySpinData uiTemplateLuckySpinData, InternetService internetService, UITemplateInventoryDataController uiTemplateInventoryDataController)
        {
            this.uiTemplateLuckySpinData           = uiTemplateLuckySpinData;
            this.internetService                   = internetService;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
        }
        
        private int CurrentFreeTurns
        {
            get => this.uiTemplateInventoryDataController.GetCurrencyValue(UITemplateInventoryDataController.DefaultLuckySpinFreeTurnCurrencyID);
            set => this.uiTemplateInventoryDataController.UpdateCurrency(value, UITemplateInventoryDataController.DefaultLuckySpinFreeTurnCurrencyID);
        }

        public void CheckGetFreeTurn()
        {
            var isDiffDay = this.internetService.IsDifferentDay(this.uiTemplateLuckySpinData.LastTimeGetFreeTurn, DateTime.Now);

            if (isDiffDay)
            {
                this.CurrentFreeTurns++;
                this.uiTemplateLuckySpinData.LastTimeGetFreeTurn = DateTime.Now;
            }
        }

        public void SaveTimeSpinToDay()
        {
            this.uiTemplateLuckySpinData.IsFirstTimeOpenLuckySpin = true;

            if (!this.IsTurnFree())
            {
                return;
            }

            this.CurrentFreeTurns--;
            this.uiTemplateLuckySpinData.LastSpinTime   = DateTime.Now;
        }

        public bool IsTurnFree()
        {
            return this.CurrentFreeTurns > 0;
        }
    }
}