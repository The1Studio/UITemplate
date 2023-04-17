namespace TheOneStudio.UITemplate.UITemplate.FTUE.TutorialTriggerCondition
{
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using Zenject;

    public class UITemplateDefaultStepFTUE : UITemplateFTUEStepBase
    {
        public override string StepId => "DefaultStep";

        public UITemplateDefaultStepFTUE(ILogService logger, SignalBus signalBus, UITemplateFTUEBlueprint uiTemplateFtueBlueprint, UITemplateFTUEControllerData uiTemplateFtueControllerData,
            UITemplateFTUEController uiTemplateFtueController) : base(logger, signalBus, uiTemplateFtueBlueprint, uiTemplateFtueControllerData, uiTemplateFtueController)
        {
        }

        public override bool IsPassedCondition() { return true; }
    }
}