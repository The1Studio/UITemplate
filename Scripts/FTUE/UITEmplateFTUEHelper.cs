namespace TheOneStudio.UITemplate.UITemplate.FTUE
{
    using System.Linq;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;

    public class UITEmplateFTUEHelper
    {
        private readonly ScreenManager                screenManager;
        private readonly UITemplateFTUEBlueprint      ftueBlueprint;
        private readonly UITemplateFTUEControllerData uiTemplateFtueControllerData;

        public UITEmplateFTUEHelper(ScreenManager screenManager,UITemplateFTUEBlueprint ftueBlueprint,UITemplateFTUEControllerData uiTemplateFtueControllerData)
        {
            this.screenManager                = screenManager;
            this.ftueBlueprint                = ftueBlueprint;
            this.uiTemplateFtueControllerData = uiTemplateFtueControllerData;
        }

        public bool IsAnyFtueActive()
        {
            var currentScreen = this.screenManager.CurrentActiveScreen.GetType().Name;

            foreach (var ftue in this.ftueBlueprint.Where(x => x.Value.EnableTrigger))
            {
                if (!currentScreen.Equals(ftue.Value.ScreenLocation)) continue;
                var isCompleteAllRequire = this.uiTemplateFtueControllerData.IsCompleteAllRequireCondition(ftue.Value.RequireCondition);

                if (!isCompleteAllRequire || this.uiTemplateFtueControllerData.IsFinishedStep(ftue.Value.Id)) continue;

                return true;
            }

            return false;
        }
    }
}