namespace TheOneStudio.UITemplate.UITemplate.FTUE
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using GameFoundation.DI;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.Utilities.Extension;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TheOneStudio.UITemplate.UITemplate.FTUE.Conditions;
    using TheOneStudio.UITemplate.UITemplate.FTUE.RemoteConfig;
    using TheOneStudio.UITemplate.UITemplate.FTUE.Signal;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine;
    using UnityEngine.Scripting;

    public class UITemplateFTUESystem : IInitializable
    {
        #region inject

        private readonly SignalBus                          signalBus;
        private readonly UITemplateFTUEDataController       uiTemplateFtueDataController;
        private readonly UITemplateFTUEBlueprintDataHandler uiTemplateFtueBlueprint;
        private readonly UITemplateFTUEController           uiTemplateFtueController;
        private readonly IScreenManager                     screenManager;

        #endregion

        private Dictionary<string, IFtueCondition>      IDToFtueConditions                      { get; }
        private Dictionary<string, HashSet<GameObject>> StepIdToEnableOnAndAfterFTUEGameObjects { get; } = new();
        private Dictionary<string, HashSet<GameObject>> StepIdToShowOnFTUEGameObjects           { get; } = new();
        private Dictionary<string, HashSet<GameObject>> StepIdToShowBeforeFTUEGameObjects       { get; } = new();
        private CancellationTokenSource                 cancellationTokenSource = new();

        [Preserve]
        public UITemplateFTUESystem(
            SignalBus                          signalBus,
            UITemplateFTUEDataController       uiTemplateFtueDataController,
            UITemplateFTUEBlueprintDataHandler uiTemplateFtueBlueprint,
            UITemplateFTUEController           uiTemplateFtueController,
            IScreenManager                     screenManager,
            List<IFtueCondition>               ftueConditions
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
            this.cancellationTokenSource.Cancel();
            this.uiTemplateFtueDataController.CompleteStep(obj.StepId);
            var disableObjectSet = this.StepIdToShowOnFTUEGameObjects.GetOrAdd(obj.StepId, () => new HashSet<GameObject>());
            foreach (var gameObject in disableObjectSet) gameObject.SetActive(false);
            this.uiTemplateFtueController.DoDeactiveFTUE(obj.StepId);
            var nextStepId = this.uiTemplateFtueBlueprint[obj.StepId].NextStepId;
            if (!nextStepId.IsNullOrEmpty()) this.OnTriggerFTUE(new(nextStepId));
        }

        [Obsolete("Use RegisterEnableOnAndAfterFTUEObjectToStepId instead")]
        public void RegisterEnableObjectToStepId(GameObject gameObject, string stepId)
        {
            var objectSet = this.StepIdToEnableOnAndAfterFTUEGameObjects.GetOrAdd(stepId, () => new HashSet<GameObject>());
            objectSet.Add(gameObject);
            gameObject.SetActive(this.uiTemplateFtueDataController.IsFinishedStep(stepId) || this.IsFTUEActiveAble(stepId));
        }

        public void RegisterEnableOnAndAfterFTUEObjectToStepId(GameObject gameObject, string stepId)
        {
            var objectSet = this.StepIdToEnableOnAndAfterFTUEGameObjects.GetOrAdd(stepId, () => new HashSet<GameObject>());
            objectSet.Add(gameObject);
            gameObject.SetActive(this.uiTemplateFtueDataController.IsFinishedStep(stepId) || this.IsFTUEActiveAble(stepId));
        }

        [Obsolete("Use RegisterOnlyShowOnFTUEObjectToStepId instead")]
        public void RegisterDisableObjectToStepId(GameObject gameObject, string stepId)
        {
            gameObject.SetActive(false);
            var objectSet = this.StepIdToShowOnFTUEGameObjects.GetOrAdd(stepId, () => new HashSet<GameObject>());
            objectSet.Add(gameObject);
        }

        public void RegisterShowOnFTUEObjectToStepId(GameObject gameObject, string stepId)
        {
            gameObject.SetActive(false);
            var objectSet = this.StepIdToShowOnFTUEGameObjects.GetOrAdd(stepId, () => new HashSet<GameObject>());
            objectSet.Add(gameObject);
        }

        public void RegisterShowBeforeFTUEObjectToStepId(GameObject gameObject, string stepId)
        {
            gameObject.SetActive(!(this.uiTemplateFtueDataController.IsFinishedStep(stepId) || this.IsFTUEActiveAble(stepId)));
            var objectSet = this.StepIdToShowBeforeFTUEGameObjects.GetOrAdd(stepId, () => new HashSet<GameObject>());
            objectSet.Add(gameObject);
        }

        private void OnTriggerFTUE(FTUETriggerSignal obj)
        {
            this.cancellationTokenSource = new();
            var stepId = obj.StepId;

            if (stepId.IsNullOrEmpty()) return;
            if (this.uiTemplateFtueController.ThereIsFTUEActive()) return;

            var enableObjectSet = this.StepIdToEnableOnAndAfterFTUEGameObjects.GetOrAdd(stepId, () => new HashSet<GameObject>());
            if (!this.IsFTUEActiveAble(stepId))
            {
                foreach (var gameObject in enableObjectSet) gameObject.SetActive(this.uiTemplateFtueDataController.IsFinishedStep(stepId));

                return;
            }
            foreach (var gameObject in enableObjectSet) gameObject.SetActive(true);

            var disableObjectSet = this.StepIdToShowBeforeFTUEGameObjects.GetOrAdd(obj.StepId, () => new HashSet<GameObject>());
            foreach (var gameObject in disableObjectSet) gameObject.SetActive(false);

            disableObjectSet = this.StepIdToShowOnFTUEGameObjects.GetOrAdd(stepId, () => new HashSet<GameObject>());
            foreach (var disableObject in disableObjectSet) disableObject.SetActive(true);

            if (!this.uiTemplateFtueDataController.IsRewardedStep(stepId))
            {
                this.uiTemplateFtueDataController.GiveReward(stepId);
            }
            this.uiTemplateFtueController.DoActiveFTUE(stepId);
            var duration = this.uiTemplateFtueBlueprint.GetDataById(stepId).TooltipDuration;
            if (duration > 0)
            {
                UniTask.Delay(
                    TimeSpan.FromSeconds(this.uiTemplateFtueBlueprint.GetDataById(stepId).TooltipDuration),
                    cancellationToken: this.cancellationTokenSource.Token).ContinueWith(() =>
                {
                    this.signalBus.Fire(new FTUEDoActionSignal(stepId));
                }).Forget();
            }
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

        public string GetTooltipText(string stepId)
        {
            return this.uiTemplateFtueBlueprint.GetDataById(stepId).TooltipText;
        }

        public string GetConditionTooltipText(string stepId)
        {
            var requireConditions = this.uiTemplateFtueBlueprint.GetDataById(stepId).GetRequireCondition();
            return requireConditions is { Count: > 0 } ? this.IDToFtueConditions[requireConditions.First().RequireId].GetTooltipText(requireConditions.First().ConditionDetail) : "";
        }

        public bool IsAnyFtueActive()
        {
            return this.IsAnyFtueActive(this.screenManager.CurrentActiveScreen.Value);
        }

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