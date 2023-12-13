namespace TheOneStudio.UITemplate.Quests.Data.Conditions
{
    using System;
    using Newtonsoft.Json;
    using UnityEngine;

    public class CompleteQuestCondition : ICondition
    {
        [JsonProperty] [field: SerializeField] private string QuestId { get; set; }

        ICondition.IProgress ICondition.SetupProgress() => new Progress();

        private class Progress : ICondition.IProgress
        {
            Type ICondition.IProgress.HandlerType => typeof(Handler);

            private class Handler : ConditionProgressHandler<CompleteQuestCondition, Progress>
            {
                private readonly UITemplateQuestManager questManager;

                public Handler(UITemplateQuestManager questManager)
                {
                    this.questManager = questManager;
                }

                public override float CurrentProgress => this.otherQuest.Progress.Status.HasFlag(QuestStatus.Completed) ? 1f : 0f;
                public override float MaxProgress     => 1f;

                private UITemplateQuestController otherQuest;

                public override void Initialize()
                {
                    this.otherQuest = this.questManager.GetController(this.Condition.QuestId);
                }
            }
        }
    }
}