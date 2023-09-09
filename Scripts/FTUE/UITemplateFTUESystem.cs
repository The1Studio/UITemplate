namespace TheOneStudio.UITemplate.UITemplate.FTUE
{
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TheOneStudio.UITemplate.UITemplate.FTUE.Conditions;
    using TheOneStudio.UITemplate.UITemplate.FTUE.Signal;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using Zenject;

    public class UITemplateFTUESystem : IInitializable
    {
        #region inject

        private readonly SignalBus                    signalBus;
        private readonly UITemplateFTUEControllerData uiTemplateFtueControllerData;
        private readonly UITemplateFTUEBlueprint      ftueBlueprint;
        private readonly UITemplateFTUEController     uiTemplateFtueController;
        private readonly IScreenManager               screenManager;

        #endregion

        private Dictionary<string, IFtueCondition> idToFtueConditions { get; }

        public UITemplateFTUESystem(SignalBus signalBus,
            UITemplateFTUEControllerData uiTemplateFtueControllerData,
            UITemplateFTUEBlueprint ftueBlueprint, UITemplateFTUEController uiTemplateFtueController, IScreenManager screenManager, List<IFtueCondition> ftueConditions)
        {
            this.signalBus                    = signalBus;
            this.uiTemplateFtueControllerData = uiTemplateFtueControllerData;
            this.ftueBlueprint                = ftueBlueprint;
            this.uiTemplateFtueController     = uiTemplateFtueController;
            this.screenManager                = screenManager;
            this.idToFtueConditions           = ftueConditions.ToDictionary(condition => condition.Id, condition => condition);

        }

        public void Initialize()
        {
            this.signalBus.Subscribe<FTUETriggerSignal>(this.OnTriggerFTUE);
            this.signalBus.Subscribe<FTUEButtonClickSignal>(this.OnFTUEButtonClick);
        }

        private void OnFTUEButtonClick(FTUEButtonClickSignal obj)
        {
            this.uiTemplateFtueController.DisableTutorial();
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

            this.uiTemplateFtueController.PrepareTutorial(obj.StepId);
        }

        private bool IsPassedStepCondition(string stepId)
        {
            var ftueRecord = this.ftueBlueprint.GetDataById(stepId);

            return ftueRecord.GetRequireCondition().All(requireCondition => this.idToFtueConditions[requireCondition.RequireId].IsPassedCondition(requireCondition.ConditionDetail));
        }

        public bool IsAnyFtueActive() => this.IsAnyFtueActive(this.screenManager.CurrentActiveScreen.Value);

        public bool IsAnyFtueActive(IScreenPresenter screenPresenter)
        {
            var currentScreen = screenPresenter.GetType().Name;

            foreach (var stepBlueprintRecord in this.ftueBlueprint.Values)
            {
                if (!currentScreen.Equals(stepBlueprintRecord.ScreenLocation)) continue;

                if (!this.uiTemplateFtueControllerData.IsCompleteAllRequireCondition(stepBlueprintRecord.RequireTriggerComplete)) continue;
                if (this.uiTemplateFtueControllerData.IsFinishedStep(stepBlueprintRecord.Id)) continue;

                var isPassedCondition = this.IsPassedStepCondition(stepBlueprintRecord.Id);

                if (!isPassedCondition) continue;

                return true;
            }

            return false;
        }
    }
}