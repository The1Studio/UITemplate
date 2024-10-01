namespace TheOneStudio.UITemplate.UITemplate.FTUE
{
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.DI;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.Utilities.Extension;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TheOneStudio.UITemplate.UITemplate.FTUE.Conditions;
    using TheOneStudio.UITemplate.UITemplate.FTUE.Signal;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine;
    using UnityEngine.Scripting;

    public class UITemplateFTUESystem : IInitializable
    {
        #region inject

        private readonly SignalBus                    signalBus;
        private readonly UITemplateFTUEDataController uiTemplateFtueDataController;
        private readonly UITemplateFTUEBlueprint      uiTemplateFtueBlueprint;
        private readonly UITemplateFTUEController     uiTemplateFtueController;
        private readonly IScreenManager               screenManager;

        #endregion

        private Dictionary<string, IFtueCondition>      IDToFtueConditions         { get; }
        private Dictionary<string, HashSet<GameObject>> StepIdToEnableGameObjects  { get; } = new(); //Use to enable the UI follow user's FTUE
        private Dictionary<string, HashSet<GameObject>> StepIdToDisableGameObjects { get; } = new(); //Use to disable the UI follow user's FTUE

        [Preserve]
        public UITemplateFTUESystem(
            SignalBus                    signalBus,
            UITemplateFTUEDataController uiTemplateFtueDataController,
            UITemplateFTUEBlueprint      uiTemplateFtueBlueprint,
            UITemplateFTUEController     uiTemplateFtueController,
            IScreenManager               screenManager,
            List<IFtueCondition>         ftueConditions
        )
        {
            this.signalBus                    = signalBus;
            this.uiTemplateFtueDataController = uiTemplateFtueDataController;
            this.uiTemplateFtueBlueprint      = uiTemplateFtueBlueprint;
            this.uiTemplateFtueController     = uiTemplateFtueController;
            this.screenManager                = screenManager;
            this.IDToFtueConditions           = ftueConditions.ToDictionary(condition => condition.Id, condition => condition);
        }

        public void Initialize()
        {
            this.signalBus.Subscribe<FTUETriggerSignal>(this.OnTriggerFTUE);
            this.signalBus.Subscribe<FTUEButtonClickSignal>(this.OnFTUEStepFinishedHandler);
            this.signalBus.Subscribe<FTUEDoActionSignal>(this.OnFTUEStepFinishedHandler);
        }

        //TODO : need to refactor for contunious FTUE
        public void OnFTUEStepFinishedHandler(IHaveStepId obj)
        {
            this.uiTemplateFtueDataController.CompleteStep(obj.StepId);
            var disableObjectSet = this.StepIdToDisableGameObjects.GetOrAdd(obj.StepId, () => new HashSet<GameObject>());
            foreach (var gameObject in disableObjectSet)
            {
                gameObject.SetActive(false);
            }
            this.uiTemplateFtueController.DoDeactiveFTUE(obj.StepId);
            var nextStepId = this.uiTemplateFtueBlueprint[obj.StepId].NextStepId;
            if (!nextStepId.IsNullOrEmpty())
            {
                this.OnTriggerFTUE(new FTUETriggerSignal(nextStepId));
            }
        }

        public void RegisterEnableObjectToStepId(GameObject gameObject, string stepId)
        {
            var objectSet = this.StepIdToEnableGameObjects.GetOrAdd(stepId, () => new HashSet<GameObject>());
            objectSet.Add(gameObject);
            //In the case the game object in the initialized screen
            gameObject.SetActive(this.uiTemplateFtueDataController.IsFinishedStep(stepId) || this.IsFTUEActiveAble(stepId));
        }

        public void RegisterDisableObjectToStepId(GameObject gameObject, string stepId)
        {
            gameObject.SetActive(false);
            var objectSet = this.StepIdToDisableGameObjects.GetOrAdd(stepId, () => new HashSet<GameObject>());
            objectSet.Add(gameObject);
        }

        private void OnTriggerFTUE(FTUETriggerSignal obj)
        {
            var stepId = obj.StepId;

            if (stepId.IsNullOrEmpty()) return;
            if (this.uiTemplateFtueController.ThereIsFTUEActive()) return;

            var enableObjectSet = this.StepIdToEnableGameObjects.GetOrAdd(stepId, () => new HashSet<GameObject>());
            if (!this.IsFTUEActiveAble(stepId))
            {
                foreach (var gameObject in enableObjectSet)
                {
                    gameObject.SetActive(this.uiTemplateFtueDataController.IsFinishedStep(stepId));
                }

                return;
            }

            foreach (var gameObject in enableObjectSet)
            {
                gameObject.SetActive(true);
            }

            var disableObjectSet = this.StepIdToDisableGameObjects.GetOrAdd(stepId, () => new HashSet<GameObject>());
            this.uiTemplateFtueController.DoActiveFTUE(stepId, disableObjectSet);
        }

        public bool IsFTUEActiveAble(string stepId)
        {
            if (this.uiTemplateFtueDataController.IsFinishedStep(stepId)) return false;

            if (this.uiTemplateFtueBlueprint.GetDataById(stepId).RequireTriggerComplete.Any(stepId => !this.uiTemplateFtueDataController.IsFinishedStep(stepId))) return false;

            var requireConditions = this.uiTemplateFtueBlueprint.GetDataById(stepId).GetRequireCondition();

            if (requireConditions != null && !requireConditions.All(requireCondition => this.IDToFtueConditions[requireCondition.RequireId].IsPassedCondition(requireCondition.ConditionDetail)))
                return
                    false;

            return true;
        }

        public bool IsAnyFtueActive() => this.IsAnyFtueActive(this.screenManager.CurrentActiveScreen.Value);

        public bool IsAnyFtueActive(IScreenPresenter screenPresenter)
        {
            var currentScreen = screenPresenter.GetType().Name;

            foreach (var stepBlueprintRecord in this.uiTemplateFtueBlueprint.Values)
            {
                if (!currentScreen.Equals(stepBlueprintRecord.ScreenLocation)) continue;
                if (!this.IsFTUEActiveAble(stepBlueprintRecord.Id)) continue;

                return true;
            }

            return false;
        }
    }
}