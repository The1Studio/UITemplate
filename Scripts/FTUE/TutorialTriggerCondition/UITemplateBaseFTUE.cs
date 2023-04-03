namespace TheOneStudio.UITemplate.UITemplate.FTUE.TutorialTriggerCondition
{
    using System;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.FTUE.Signal;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using Zenject;

    public interface IUITemplateFTUE
    {
        string TriggerId { get; }
        void   Execute();
    }

    public abstract class UITemplateBaseFTUE : IUITemplateFTUE, IInitializable, IDisposable
    {
        protected readonly ILogService                  Logger;
        protected readonly SignalBus                    SignalBus;
        private readonly   UITemplateFTUEControllerData uiTemplateFtueControllerData;
        protected readonly UITemplateFTUEController     UITemplateFtueController;
        protected readonly ScreenManager                ScreenManager;
        public abstract    string                       TriggerId { get; }

        protected UITemplateBaseFTUE(ILogService logger, SignalBus signalBus, UITemplateFTUEControllerData uiTemplateFtueControllerData, UITemplateFTUEController uiTemplateFtueController,
            ScreenManager screenManager)
        {
            this.Logger                       = logger;
            this.SignalBus                    = signalBus;
            this.uiTemplateFtueControllerData = uiTemplateFtueControllerData;
            this.UITemplateFtueController     = uiTemplateFtueController;
            this.ScreenManager                = screenManager;
        }

        public void Initialize() { this.SignalBus.Subscribe<FTUEButtonClickSignal>(this.OnFTUEButtonClick); }

        protected abstract UniTask<bool> PreProcess();

        public async void Execute()
        {
            var canTrigger = await this.PreProcess();

            if (!canTrigger)
            {
                this.UITemplateFtueController.SetTutorialStatus(false, this.TriggerId);
            }
            else
            {
                this.UITemplateFtueController.SetTutorialStatus(true, this.TriggerId);
            }
        }

        protected         void SaveCompleteStepToLocalData()                { this.uiTemplateFtueControllerData.CompleteStep(this.TriggerId); }

        protected virtual void OnFTUEButtonClick(FTUEButtonClickSignal obj)
        {
            this.UITemplateFtueController.SetTutorialStatus(false, this.TriggerId);
        }

        public void Dispose() { this.SignalBus.Unsubscribe<FTUEButtonClickSignal>(this.OnFTUEButtonClick); }
    }
}