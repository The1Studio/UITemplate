namespace TheOneStudio.UITemplate.UITemplate.FTUE
{
    using System.Linq;
    using System.Runtime;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;

    public class UITemplateFTUEHelper
    {
        private readonly ScreenManager                screenManager;
        private readonly UITemplateFTUEBlueprint      ftueBlueprint;
        private readonly UITemplateFTUEControllerData uiTemplateFtueControllerData;

        public UITemplateFTUEHelper(ScreenManager screenManager,UITemplateFTUEBlueprint ftueBlueprint,UITemplateFTUEControllerData uiTemplateFtueControllerData)
        {
            this.screenManager                = screenManager;
            this.ftueBlueprint                = ftueBlueprint;
            this.uiTemplateFtueControllerData = uiTemplateFtueControllerData;
        }

        public bool IsAnyFtueActive() => this.IsAnyFtueActive(this.screenManager.CurrentActiveScreen.Value);

        public bool IsAnyFtueActive(IScreenPresenter screenPresenter)
        {
            var currentScreen = screenPresenter.GetType().Name;

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