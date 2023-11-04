namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using System;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using Zenject;

    public class UITemplateGameSessionDataController : IUITemplateControllerData, IInitializable
    {
        private readonly UITemplateGameSessionData gameSessionData;

        public UITemplateGameSessionDataController(UITemplateGameSessionData gameSessionData) { this.gameSessionData = gameSessionData; }

        public DateTime FirstInstallDate => this.gameSessionData.FirstInstallDate;
        public int OpenTime => this.gameSessionData.OpenTime;
        
        public void Initialize()
        {
            this.gameSessionData.OpenTime++;
        }
    }
}