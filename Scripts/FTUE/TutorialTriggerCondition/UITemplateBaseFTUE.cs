namespace TheOneStudio.UITemplate.UITemplate.FTUE.TutorialTriggerCondition
{
    using System;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.FTUE.Signal;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using Zenject;

    public interface IUITemplateFTUE
    {
        string StepId { get; }
        bool   IsPassedCondition();
        void   Execute();
    }

    public abstract class UITemplateFTUEStepBase : IUITemplateFTUE, IInitializable, IDisposable
    {
        protected readonly ILogService                  Logger;
        protected readonly SignalBus                    SignalBus;
        private readonly   UITemplateFTUEControllerData uiTemplateFtueControllerData;
        protected readonly UITemplateFTUEController     UITemplateFtueController;
        public abstract    string                       StepId { get; }

        protected UITemplateFTUEStepBase(ILogService   logger, SignalBus signalBus, UITemplateFTUEControllerData uiTemplateFtueControllerData, UITemplateFTUEController uiTemplateFtueController)
        {
            this.Logger                       = logger;
            this.SignalBus                    = signalBus;
            this.uiTemplateFtueControllerData = uiTemplateFtueControllerData;
            this.UITemplateFtueController     = uiTemplateFtueController;
        }

        public void Initialize() { this.SignalBus.Subscribe<FTUEButtonClickSignal>(this.OnFTUEButtonClick); }

        public abstract bool IsPassedCondition();

        public void Execute()
        {
            var canTrigger = this.IsPassedCondition();

            if (!canTrigger)
            {
                this.UITemplateFtueController.SetTutorialStatus(false, this.StepId);
            }
            else
            {
                this.UITemplateFtueController.SetTutorialStatus(true, this.StepId);
            }
        }

        protected void SaveCompleteStepToLocalData() { this.uiTemplateFtueControllerData.CompleteStep(this.StepId); }

        protected virtual void OnFTUEButtonClick(FTUEButtonClickSignal obj) { this.UITemplateFtueController.SetTutorialStatus(false, this.StepId); }

        public void Dispose() { this.SignalBus.Unsubscribe<FTUEButtonClickSignal>(this.OnFTUEButtonClick); }
    }
}