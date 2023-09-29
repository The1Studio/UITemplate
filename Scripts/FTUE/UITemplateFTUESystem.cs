namespace TheOneStudio.UITemplate.UITemplate.FTUE
{
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TheOneStudio.UITemplate.UITemplate.FTUE.Conditions;
    using TheOneStudio.UITemplate.UITemplate.FTUE.Signal;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine;
    using Zenject;

    public class UITemplateFTUESystem : IInitializable
    {
        #region inject

        private readonly SignalBus                    signalBus;
        private readonly UITemplateFTUEDataController uiTemplateFtueDataController;
        private readonly UITemplateFTUEBlueprint      uiTemplateFtueBlueprint;
        private readonly UITemplateFTUEController     uiTemplateFtueController;
        private readonly IScreenManager               screenManager;

        #endregion

        private Dictionary<string, IFtueCondition>      idToFtueConditions  { get; }
        private Dictionary<string, HashSet<GameObject>> stepIdToGameObjects { get; } = new();

        public UITemplateFTUESystem(SignalBus signalBus,
            UITemplateFTUEDataController uiTemplateFtueDataController,
            UITemplateFTUEBlueprint uiTemplateFtueBlueprint, UITemplateFTUEController uiTemplateFtueController, IScreenManager screenManager, List<IFtueCondition> ftueConditions)
        {
            this.signalBus                    = signalBus;
            this.uiTemplateFtueDataController = uiTemplateFtueDataController;
            this.uiTemplateFtueBlueprint                = uiTemplateFtueBlueprint;
            this.uiTemplateFtueController     = uiTemplateFtueController;
            this.screenManager                = screenManager;
            this.idToFtueConditions           = ftueConditions.ToDictionary(condition => condition.Id, condition => condition);

        }

        public void Initialize()
        {
            this.signalBus.Subscribe<FTUETriggerSignal>(this.OnTriggerFTUE);
            this.signalBus.Subscribe<FTUEButtonClickSignal>(this.OnFTUEButtonClick); 
        }

        //TODO : need to refactor for contunious FTUE
        private void OnFTUEButtonClick(FTUEButtonClickSignal obj)
        {
            this.uiTemplateFtueDataController.CompleteStep(obj.StepId);
            var nextStepId = this.uiTemplateFtueBlueprint[obj.StepId].NextStepId;

            if (!nextStepId.IsNullOrEmpty())
            {
                this.OnTriggerFTUE(new FTUETriggerSignal(nextStepId));
            }
            else
            {
                this.uiTemplateFtueController.DisableTutorial(obj.StepId);
            }
        }

        public void RegisterObjectToStepId(GameObject gameObject, string stepId)
        {
            var objectSet = this.stepIdToGameObjects.GetOrAdd(stepId, () => new HashSet<GameObject>());
            objectSet.Add(gameObject);
        }

        private void OnTriggerFTUE(FTUETriggerSignal obj)
        {
            var stepId = obj.StepId;
            if (stepId.IsNullOrEmpty()) return;
            
            var objectSet = this.stepIdToGameObjects.GetOrAdd(stepId, () => new HashSet<GameObject>());
            if (!this.IsFTUEActiveAble(stepId))
            {
                foreach (var gameObject in objectSet)
                {
                    gameObject.SetActive(this.uiTemplateFtueDataController.IsFinishedStep(stepId));
                }
                return;
            }
            
            foreach (var gameObject in objectSet)
            {
                gameObject.SetActive(true);
            }  

            this.uiTemplateFtueController.DoActiveFTUE(stepId);
        }

        public bool IsFTUEActiveAble(string stepId)
        {
            if (this.uiTemplateFtueDataController.IsFinishedStep(stepId)) return false;

            if (!this.uiTemplateFtueDataController.IsCompleteAllRequireCondition(this.uiTemplateFtueBlueprint.GetDataById(stepId).RequireTriggerComplete)) return false;
            
            if (!this.uiTemplateFtueBlueprint.GetDataById(stepId).GetRequireCondition().All(requireCondition => this.idToFtueConditions[requireCondition.RequireId].IsPassedCondition(requireCondition.ConditionDetail))) return
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