namespace TheOneStudio.UITemplate.UITemplate.FTUE
{
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.FTUE.TutorialTriggerCondition;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using Zenject;

    public class UITemplateFTUESystem : IInitializable
    {
        private readonly SignalBus                           signalBus;
        private readonly UITemplateFTUEControllerData        uiTemplateFtueControllerData;
        private readonly List<IUITemplateFTUE>               uiTemplateBaseFtues;
        private readonly UITemplateFTUEBlueprint             ftueBlueprint;
        private readonly ScreenManager                       screenManager;
        private readonly UITemplateFTUEController            uiTemplateFtueController;
        private          Dictionary<string, IUITemplateFTUE> dicUITemplateFTUE = new();

        public UITemplateFTUESystem(SignalBus signalBus, UITemplateFTUEControllerData uiTemplateFtueControllerData, List<IUITemplateFTUE> uiTemplateBaseFtues, UITemplateFTUEBlueprint ftueBlueprint,
            ScreenManager screenManager, UITemplateFTUEController uiTemplateFtueController)
        {
            this.signalBus                    = signalBus;
            this.uiTemplateFtueControllerData = uiTemplateFtueControllerData;
            this.uiTemplateBaseFtues          = uiTemplateBaseFtues;
            this.ftueBlueprint                = ftueBlueprint;
            this.screenManager                = screenManager;
            this.uiTemplateFtueController     = uiTemplateFtueController;
        }

        public void Initialize()
        {
            foreach (var u in this.uiTemplateBaseFtues)
            {
                this.dicUITemplateFTUE.Add(u.TriggerId, u);
            }

            this.signalBus.Subscribe<StartLoadingNewSceneSignal>(this.OnStartLoadingNewScene);
            this.signalBus.Subscribe<FinishLoadingNewSceneSignal>(this.OnFinishLoadingNewScene);
            this.signalBus.Subscribe<ScreenShowSignal>(this.OnScreenShow);
        }

        private void OnFinishLoadingNewScene(FinishLoadingNewSceneSignal obj) { this.uiTemplateFtueController.MoveToCurrentRootUI(this.screenManager.CurrentOverlayRoot); }

        private void OnStartLoadingNewScene(StartLoadingNewSceneSignal obj) { this.uiTemplateFtueController.MoveToOriginParent(); }

        private void OnScreenShow(ScreenShowSignal obj)
        {
            foreach (var ftue in this.ftueBlueprint.Where(x => x.Value.EnableTrigger))
            {
                if (!obj.ScreenPresenter.GetType().Name.Equals(ftue.Value.ScreenLocation)) continue;
                var isCompleteAllRequire = this.uiTemplateFtueControllerData.IsCompleteAllRequireCondition(ftue.Value.RequireCondition);

                //CompleteAll Requie Condition?
                if (!isCompleteAllRequire || this.uiTemplateFtueControllerData.IsFinishedStep(ftue.Value.Id)) continue;
                this.dicUITemplateFTUE[ftue.Value.Id].Execute();

                return;
            }
        }
    }
}