namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.Utilities.Extension;
    using GameFoundation.Scripts.Utilities.UserData;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using Zenject;

    public class UITemplateGameSessionDataController : IUITemplateControllerData
    {
        private readonly UITemplateGameSessionData gameSessionData;

        public UITemplateGameSessionDataController(UITemplateGameSessionData gameSessionData) { this.gameSessionData = gameSessionData; }

        public DateTime FirstInstallDate => this.gameSessionData.FirstInstallDate;
    }
}