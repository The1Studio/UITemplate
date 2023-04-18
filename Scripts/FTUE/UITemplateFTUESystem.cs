namespace TheOneStudio.UITemplate.UITemplate.FTUE
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TheOneStudio.UITemplate.UITemplate.FTUE.Signal;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using Zenject;

    public class UITemplateFTUESystem : IInitializable
    {
        private readonly SignalBus                    signalBus;
        private readonly UITemplateFTUEHelper         uiTemplateFtueHelper;
        private readonly UITemplateFTUEControllerData uiTemplateFtueControllerData;
        private readonly UITemplateFTUEBlueprint      ftueBlueprint;
        private readonly UITemplateFTUEController     uiTemplateFtueController;

        public UITemplateFTUESystem(SignalBus signalBus, UITemplateFTUEHelper uiTemplateFtueHelper,
            UITemplateFTUEControllerData uiTemplateFtueControllerData,
            UITemplateFTUEBlueprint ftueBlueprint,
            ScreenManager screenManager, UITemplateFTUEController uiTemplateFtueController)
        {
            this.signalBus                    = signalBus;
            this.uiTemplateFtueHelper         = uiTemplateFtueHelper;
            this.uiTemplateFtueControllerData = uiTemplateFtueControllerData;
            this.ftueBlueprint                = ftueBlueprint;
            this.uiTemplateFtueController     = uiTemplateFtueController;
        }

        public void Initialize()
        {
            this.signalBus.Subscribe<FTUETriggerSignal>(this.OnTriggerFTUE);
            this.signalBus.Subscribe<FTUEButtonClickSignal>(this.OnFTUEButtonClick);
        }

        private void OnFTUEButtonClick(FTUEButtonClickSignal obj)
        {
            this.uiTemplateFtueController.SetTutorialStatus(false, obj.StepId);
            this.uiTemplateFtueControllerData.CompleteStep(obj.StepId);
            var nextStepId = this.ftueBlueprint[obj.StepId].NextStepId;

            if (nextStepId.IsNullOrEmpty()) return;
            this.OnTriggerFTUE(new FTUETriggerSignal(nextStepId));
        }

        private void OnTriggerFTUE(FTUETriggerSignal obj)
        {
            if (obj.StepId.IsNullOrEmpty()) return;
            var isCompleteAllRequire = this.uiTemplateFtueControllerData.IsCompleteAllRequireCondition(this.ftueBlueprint[obj.StepId].RequireTriggerComplete);

            if (!isCompleteAllRequire || this.uiTemplateFtueControllerData.IsFinishedStep(obj.StepId)) return;
            this.Execute(obj.StepId);
        }

        private void Execute(string stepId)
        {
            var canTrigger = this.uiTemplateFtueHelper.IsPassedCondition(stepId);

            this.uiTemplateFtueController.SetTutorialStatus(canTrigger, stepId);
        }
    }
}