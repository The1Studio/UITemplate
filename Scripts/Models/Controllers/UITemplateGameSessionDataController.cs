namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using System;
    using GameFoundation.DI;
    using GameFoundation.Scripts.Utilities.UserData;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using UnityEngine.Scripting;

    public class UITemplateGameSessionDataController : IUITemplateControllerData, IInitializable
    {
        private readonly UITemplateGameSessionData gameSessionData;
        private readonly SignalBus                 signalBus;

        [Preserve]
        public UITemplateGameSessionDataController(UITemplateGameSessionData gameSessionData, SignalBus signalBus)
        {
            this.gameSessionData = gameSessionData;
            this.signalBus       = signalBus;
        }

        public DateTime FirstInstallDate => this.gameSessionData.FirstInstallDate;
        public int OpenTime => this.gameSessionData.OpenTime;

        public void Initialize()
        {
            this.signalBus.Subscribe<UserDataLoadedSignal>(this.OnUserDataLoadedHandler);
        }

        private void OnUserDataLoadedHandler()
        {
            this.gameSessionData.OpenTime++;
        }
    }
}