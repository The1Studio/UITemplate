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

        public UITemplateFTUEHelper(ScreenManager screenManager, UITemplateFTUEBlueprint ftueBlueprint, UITemplateFTUEControllerData uiTemplateFtueControllerData,
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

            foreach (var stepBlueprintRecord in this.ftueBlueprint.Values)
            {
                if (!currentScreen.Equals(stepBlueprintRecord.ScreenLocation)) continue;

                if (!this.uiTemplateFtueControllerData.IsCompleteAllRequireCondition(stepBlueprintRecord.RequireCondition)) continue;
                if (this.uiTemplateFtueControllerData.IsFinishedStep(stepBlueprintRecord.Id)) continue;
                var isPassedCondition = false;

                if (stepBlueprintRecord.AutoPassCondition)
                {
                    isPassedCondition = true;
                }
                else
                {
                    var step = this.uiTemplateSteps.Find(x => x.StepId == stepBlueprintRecord.Id);

                    if (step != null)
                        isPassedCondition = step.IsPassedCondition();
                }

                if (!isPassedCondition) continue;

                return true;
            }

            return false;
        }
    }
}