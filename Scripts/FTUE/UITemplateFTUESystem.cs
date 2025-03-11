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

        #region Register Objects

        public void RegisterEnableOnAndAfterFTUEObjectToStepId(GameObject gameObject, string stepId)
        {
            var objectSet = this.StepIdToEnableOnAndAfterFTUEGameObjects.GetOrAdd(stepId, () => new HashSet<GameObject>());
            objectSet.Add(gameObject);
            gameObject.SetActive(this.IsOnFTUE(stepId) || this.IsAfterFTUE(stepId));
        }

        public void RegisterShowOnFTUEObjectToStepId(GameObject gameObject, string stepId)
        {
            gameObject.SetActive(false);
            var objectSet = this.StepIdToShowOnFTUEGameObjects.GetOrAdd(stepId, () => new HashSet<GameObject>());
            objectSet.Add(gameObject);
        }

        public void RegisterShowBeforeFTUEObjectToStepId(GameObject gameObject, string stepId)
        {
            gameObject.SetActive(this.IsBeforeFTUE(stepId));
            var objectSet = this.StepIdToShowBeforeFTUEGameObjects.GetOrAdd(stepId, () => new HashSet<GameObject>());
            objectSet.Add(gameObject);
        }

        #endregion

        #region Signal Handlers

        private void OnTriggerFTUE(FTUETriggerSignal obj)
        {
            this.cancellationTokenSource = new();
            var stepId = obj.StepId;

            if (stepId.IsNullOrEmpty()) return;
            if (this.uiTemplateFtueController.ThereIsFTUEActive()) return;

            var onAndAfterObjectSet = this.StepIdToEnableOnAndAfterFTUEGameObjects.GetOrAdd(stepId, () => new HashSet<GameObject>());
            var beforeObjectSet     = this.StepIdToShowBeforeFTUEGameObjects.GetOrAdd(obj.StepId, () => new HashSet<GameObject>());
            var onObjectSet         = this.StepIdToShowOnFTUEGameObjects.GetOrAdd(stepId, () => new HashSet<GameObject>());
            if (!this.CanTriggerFTUE(stepId))
            {
                foreach (var gameObject in onAndAfterObjectSet) gameObject.SetActive(this.IsOnFTUE(stepId) || this.IsAfterFTUE(stepId));
                foreach (var gameObject in beforeObjectSet) gameObject.SetActive(this.IsBeforeFTUE(stepId));
                foreach (var gameObject in onObjectSet) gameObject.SetActive(this.IsOnFTUE(stepId));
                return;
            }

            foreach (var gameObject in onAndAfterObjectSet) gameObject.SetActive(true);
            foreach (var gameObject in beforeObjectSet) gameObject.SetActive(false);
            foreach (var gameObject in onObjectSet) gameObject.SetActive(true);

            var record = this.uiTemplateFtueBlueprint.GetDataById(stepId);
            if (record.ShowUnlockPopup)
            {
                this.signalBus.Fire(new FTUEShowUnlockPopupSignal(
                    ShowFTUE,
                    record.ItemId,
                    "",
                    record.NextScreenName,
                    record.DestinationName));
            }
            else
            {
                ShowFTUE();
            }

            void ShowFTUE()
            {
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
            if (!nextStepId.IsNullOrEmpty())
            {
                this.OnTriggerFTUE(new(nextStepId));
            }
        }

        #endregion

        #region Ultilities

        private bool CanTriggerFTUE(string stepId)
        {
            return !this.IsFTUECompleted(stepId)
                && this.CanTrigger(stepId)
                && this.uiTemplateFtueBlueprint.GetDataById(stepId).RequireTriggerComplete.All(this.IsFTUECompleted);
        }

        private bool IsBeforeFTUE(string stepId)
        {
            return !this.IsFTUECompleted(stepId) && !this.IsFTUEPassed(stepId);
        }

        private bool IsOnFTUE(string stepId)
        {
            return this.CanTriggerFTUE(stepId);
        }

        private bool IsAfterFTUE(string stepId)
        {
            return this.IsFTUECompleted(stepId) || this.IsFTUEPassed(stepId);
        }

        private bool IsFTUEPassed(string stepId)
        {
            var requireConditions = this.uiTemplateFtueBlueprint.GetDataById(stepId).GetRequireCondition();
            return requireConditions == null || requireConditions.All(requireCondition => this.IDToFtueConditions[requireCondition.RequireId].IsPassedCondition(requireCondition.ConditionDetail));
        }

        private bool CanTrigger(string stepId)
        {
            var requireConditions = this.uiTemplateFtueBlueprint.GetDataById(stepId).GetRequireCondition();
            return requireConditions == null || requireConditions.All(requireCondition => this.IDToFtueConditions[requireCondition.RequireId].CanTrigger(requireCondition.ConditionDetail));
        }

        private bool IsFTUECompleted(string stepId)
        {
            return this.uiTemplateFtueDataController.IsFinishedStep(stepId);
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

        public string GetShortConditionText(string stepId)
        {
            var requireConditions = this.uiTemplateFtueBlueprint.GetDataById(stepId).GetRequireCondition();
            return requireConditions is { Count: > 0 } ? this.IDToFtueConditions[requireConditions.First().RequireId].GetShortConditionText(requireConditions.First().ConditionDetail) : "";
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
                if (!this.CanTriggerFTUE(stepBlueprintRecord.Id)) continue;

                return true;
            }

            return false;
        }

        #endregion
    }
}