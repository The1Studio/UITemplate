namespace TheOneStudio.UITemplate.UITemplate.FTUE
{
    using System.Collections.Generic;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.FTUE.TutorialTriggerCondition;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;

    public class UITemplateFTUEHelper
    {
        private readonly ScreenManager                screenManager;
        private readonly UITemplateFTUEBlueprint      ftueBlueprint;
        private readonly UITemplateFTUEControllerData uiTemplateFtueControllerData;
        private readonly List<IUITemplateFTUE>        uiTemplateSteps;

        public UITemplateFTUEHelper(ScreenManager                screenManager, UITemplateFTUEBlueprint ftueBlueprint, UITemplateFTUEControllerData uiTemplateFtueControllerData,
                                    List<IUITemplateFTUE> uiTemplateSteps)
        {
            this.screenManager                = screenManager;
            this.ftueBlueprint                = ftueBlueprint;
            this.uiTemplateFtueControllerData = uiTemplateFtueControllerData;
            this.uiTemplateSteps              = uiTemplateSteps;
        }

        public bool IsAnyFtueActive() => this.IsAnyFtueActive(this.screenManager.CurrentActiveScreen.Value);

        public bool IsAnyFtueActive(IScreenPresenter screenPresenter)
        {
            var currentScreen = screenPresenter.GetType().Name;
            
            foreach (var uiTemplateFtueStep in this.uiTemplateSteps)
            {
                var stepBlueprintRecord = this.ftueBlueprint.GetDataById(uiTemplateFtueStep.StepId);
                if (!currentScreen.Equals(stepBlueprintRecord.ScreenLocation)) continue;
                if (!this.uiTemplateFtueControllerData.IsCompleteAllRequireCondition(stepBlueprintRecord.RequireCondition)) continue;
                if (this.uiTemplateFtueControllerData.IsFinishedStep(stepBlueprintRecord.Id)) continue;
                if (!uiTemplateFtueStep.IsPassedCondition()) continue;

                return true;
            }

            return false;
        }
    }
}