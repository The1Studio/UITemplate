namespace TheOneStudio.UITemplate.Quests
{
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using GameFoundation.DI;
    using GameFoundation.Scripts.Utilities.Extension;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.Quests.Conditions;
    using TheOneStudio.UITemplate.Quests.Data;
    using TheOneStudio.UITemplate.Quests.Signals;
    using TheOneStudio.UITemplate.Quests.TargetHandler;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine;
    using UnityEngine.Scripting;

    public class UITemplateQuestController
    {
        #region Constructor

        private readonly IDependencyContainer                                            container;
        private readonly SignalBus                                                       signalBus;
        private readonly UITemplateInventoryDataController                               inventoryDataController;
        private readonly QuestStatusChangedSignal                                        changedSignal;
        private readonly Dictionary<ICondition.IProgress, ICondition.IProgress.IHandler> progressHandlers;

        [Preserve]
        public UITemplateQuestController(
            IDependencyContainer              container,
            SignalBus                         signalBus,
            UITemplateInventoryDataController inventoryDataController
        )
        {
            this.container               = container;
            this.signalBus               = signalBus;
            this.inventoryDataController = inventoryDataController;
            this.changedSignal           = new QuestStatusChangedSignal(this);
            this.progressHandlers        = new Dictionary<ICondition.IProgress, ICondition.IProgress.IHandler>();
        }

        #endregion

        public         QuestRecord                                           Record   { get; internal set; }
        public         UITemplateQuestProgress.Quest                         Progress { get; internal set; }
        private static Dictionary<IRedirectTarget, IRedirectTarget.IHandler> handleTargets = new Dictionary<IRedirectTarget, IRedirectTarget.IHandler>();

        public async UniTask CollectReward(RectTransform startAnimRectTransform = null)
        {
            if (this.Progress.Status is not QuestStatus.NotCollected) return;
            this.RemoveProgressHandlers(this.Progress.CompleteProgress);
            this.Progress.Status |= QuestStatus.Collected;
            await this.Record.Rewards.Select(reward => this.inventoryDataController.AddGenericReward(reward.Id, reward.Value, startAnimRectTransform));
        }

        public IEnumerable<ICondition.IProgress.IHandler> GetCompleteProgressHandlers()
        {
            return this.Progress.CompleteProgress.Select(progress => this.progressHandlers[progress]);
        }

        public IEnumerable<ICondition.IProgress.IHandler> GetAllProgressHandlers()
        {
            return this.progressHandlers.Values;
        }

        public async UniTask HandleRedirect()
        {
            if (this.Record.Target.Count is 0) return;
            foreach (var targetHandler in this.Record.Target.Select(this.AddRedirectTargetHandler))
            {
                await targetHandler.Handle();
            }
        }

        #region Internal

        internal void Initialize()
        {
            this.AddProgressHandlers(this.Record.ResetConditions, this.Progress.ResetProgress);
            if (!this.Progress.Status.HasFlag(QuestStatus.Started))
            {
                this.AddProgressHandlers(this.Record.StartConditions, this.Progress.StartProgress);
            }
            if (!this.Progress.Status.HasFlag(QuestStatus.Shown))
            {
                this.AddProgressHandlers(this.Record.ShowConditions, this.Progress.ShowProgress);
            }
            if (this.Progress.Status.HasFlag(QuestStatus.Started) && !this.Progress.Status.HasFlag(QuestStatus.Collected))
            {
                this.AddProgressHandlers(this.Record.CompleteConditions, this.Progress.CompleteProgress);
            }
        }

        internal void UpdateStatus()
        {
            if (!this.Progress.Status.HasFlag(QuestStatus.Started) && this.IsSatisfied(this.Progress.StartProgress))
            {
                this.Progress.Status |= QuestStatus.Started;
                this.RemoveProgressHandlers(this.Progress.StartProgress);
                this.AddProgressHandlers(this.Record.CompleteConditions, this.Progress.CompleteProgress);
                this.signalBus.Fire(this.changedSignal);
            }
            if (!this.Progress.Status.HasFlag(QuestStatus.Shown) && this.IsSatisfied(this.Progress.ShowProgress))
            {
                this.Progress.Status |= QuestStatus.Shown;
                this.RemoveProgressHandlers(this.Progress.ShowProgress);
                this.signalBus.Fire(this.changedSignal);
            }
            if (this.Progress.Status.HasFlag(QuestStatus.Started) && !this.Progress.Status.HasFlag(QuestStatus.Completed) && this.IsSatisfied(this.Progress.CompleteProgress))
            {
                this.Progress.Status |= QuestStatus.Completed;
                this.signalBus.Fire(this.changedSignal);
            }
        }

        internal bool CanBeReset()
        {
            return this.Record.ResetConditions.Count > 0
                && this.Progress.Status is not QuestStatus.NotCollected
                && this.IsSatisfied(this.Progress.ResetProgress);
        }

        internal void Dispose()
        {
            this.progressHandlers.Values.ForEach(controller => controller.Dispose());
            this.progressHandlers.Clear();
        }

        #endregion

        #region Private

        private bool IsSatisfied(IEnumerable<ICondition.IProgress> progresses)
        {
            return progresses.All(progress => this.progressHandlers[progress].IsSatisfied());
        }

        private IRedirectTarget.IHandler AddRedirectTargetHandler(IRedirectTarget redirectTarget)
        {
            var handler = handleTargets.GetOrAdd(redirectTarget, () => (IRedirectTarget.IHandler)this.container.Instantiate(redirectTarget.GetTypeHandle));
            handler.RedirectTarget = redirectTarget;
            return handler;
        }

        private void AddProgressHandlers(IEnumerable<ICondition> conditions, IEnumerable<ICondition.IProgress> progresses)
        {
            IterTools.StrictZip(conditions, progresses).ForEach((condition, progress) =>
            {
                if (this.progressHandlers.ContainsKey(progress)) return;
                var handler = (ICondition.IProgress.IHandler)this.container.Instantiate(progress.HandlerType);
                handler.Condition = condition;
                handler.Progress  = progress;
                handler.Initialize();
                this.progressHandlers.Add(progress, handler);
            });
        }

        private void RemoveProgressHandlers(IEnumerable<ICondition.IProgress> progresses)
        {
            progresses.ForEach(progress =>
            {
                if (!this.progressHandlers.Remove(progress, out var controller)) return;
                controller.Dispose();
            });
        }

        #endregion
    }
}