namespace TheOneStudio.UITemplate.UITemplate.FTUE.TutorialTriggerCondition
{
    using System;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TheOneStudio.UITemplate.UITemplate.FTUE.Signal;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using Zenject;

    public interface IUITemplateFTUE
    {
        string StepId { get; }
        bool   IsPassedCondition();
        void   Execute(string stepId);
    }

    public abstract class UITemplateFTUEStepBase : IUITemplateFTUE, IInitializable, IDisposable
    {
        protected readonly ILogService                  Logger;
        protected readonly SignalBus                    SignalBus;
        private readonly   UITemplateFTUEBlueprint      uiTemplateFtueBlueprint;
        private readonly   UITemplateFTUEControllerData uiTemplateFtueControllerData;
        protected readonly UITemplateFTUEController     UITemplateFtueController;
        public abstract    string                       StepId { get; }

        protected UITemplateFTUEStepBase(ILogService logger, SignalBus signalBus, UITemplateFTUEBlueprint uiTemplateFtueBlueprint, UITemplateFTUEControllerData uiTemplateFtueControllerData,
            UITemplateFTUEController uiTemplateFtueController)
        {
            this.Logger                       = logger;
            this.SignalBus                    = signalBus;
            this.uiTemplateFtueBlueprint      = uiTemplateFtueBlueprint;
            this.uiTemplateFtueControllerData = uiTemplateFtueControllerData;
            this.UITemplateFtueController     = uiTemplateFtueController;
        }

        public void Initialize() { this.SignalBus.Subscribe<FTUEButtonClickSignal>(this.OnFTUEButtonClick); }

        public abstract bool IsPassedCondition();

        public void Execute(string stepId)
        {
            var canTrigger = this.IsPassedCondition();

            this.UITemplateFtueController.SetTutorialStatus(canTrigger, stepId);
        }

        protected void SaveCompleteStepToLocalData(string stepId) { this.uiTemplateFtueControllerData.CompleteStep(stepId); }

        protected virtual void OnFTUEButtonClick(FTUEButtonClickSignal obj)
        {
            this.UITemplateFtueController.SetTutorialStatus(false, obj.StepId);
            this.SaveCompleteStepToLocalData(obj.StepId);
            this.AuoTriggerNextStep(obj.StepId);
        }

        protected void AuoTriggerNextStep(string currentStepId)
        {
            var nextStepId = this.uiTemplateFtueBlueprint[currentStepId].NextStepId;

            if (nextStepId.IsNullOrEmpty()) return;
            this.SignalBus.Fire(new FTUEManualTriggerSignal(nextStepId));
        }

        public void Dispose() { this.SignalBus.Unsubscribe<FTUEButtonClickSignal>(this.OnFTUEButtonClick); }
    }
}