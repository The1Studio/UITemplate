namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using System;
    using Cysharp.Threading.Tasks;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Services;

    public class UITemplateJackpotController:IUITemplateControllerData
    {
        private readonly InternetService                internetService;
        private readonly UITemplateUserJackpotData      userJackpotData;
        private readonly UITemplateGachaJackpotBlueprint gachaJackpotBlueprint;

        public UITemplateJackpotController(InternetService internetService, UITemplateUserJackpotData userJackpotData, UITemplateGachaJackpotBlueprint gachaJackpotBlueprint)
        {
            this.internetService      = internetService;
            this.userJackpotData      = userJackpotData;
            this.gachaJackpotBlueprint = gachaJackpotBlueprint;
        }
        
        public void DoJackpotSpin()
        {
            if (this.userJackpotData.RemainingJackpotSpin <= 0)
                return;
            
            if (this.userJackpotData.CurrentJackpotSpin >= this.gachaJackpotBlueprint.Values.Count)
                this.userJackpotData.CurrentJackpotSpin = 0;
            
            this.userJackpotData.CurrentJackpotSpin++;
            this.userJackpotData.RemainingJackpotSpin--;
            this.userJackpotData.JackpotDate = DateTime.Now;
        }
        
        public int UserCurrentJackpotSpin()
        {
            return this.userJackpotData.CurrentJackpotSpin;
        }

        public async UniTask<int> UserRemainingJackpotSpin()
        {
            if (this.userJackpotData.RemainingJackpotSpin > 0)
                return this.userJackpotData.RemainingJackpotSpin;
            
            this.userJackpotData.RemainingJackpotSpin = await this.GetUserRemainingJackpotSpinByDay();

            return this.userJackpotData.RemainingJackpotSpin;
        }

        private async UniTask<int> GetUserRemainingJackpotSpinByDay()
        {
            var currentDay      = (await this.internetService.GetCurrentTimeAsync()).Day;
            var beginDay        = this.userJackpotData.JackpotDate.Day;
            var differenceOfDay = currentDay - beginDay;

            return differenceOfDay > 0 ? 1 : 0;
        }
    }
}