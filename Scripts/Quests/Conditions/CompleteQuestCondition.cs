namespace TheOneStudio.UITemplate.Quests.Conditions
{
    using System;
    using Newtonsoft.Json;
    using TheOneStudio.UITemplate.Quests.Data;

    public sealed class CompleteQuestCondition : BaseCondition
    {
        [JsonProperty] private string QuestId { get; set; }

        protected override ICondition.IProgress SetupProgress() => new Progress();

        private sealed class Progress : BaseProgress
        {
            protected override Type HandlerType => typeof(Handler);

            private sealed class Handler : BaseHandler<CompleteQuestCondition, Progress>
            {
                private readonly UITemplateQuestManager questManager;

                public Handler(UITemplateQuestManager questManager)
                {
                    this.questManager = questManager;
                }

                protected override float CurrentProgress => this.otherQuest.Progress.Status.HasFlag(QuestStatus.Completed) ? 1f : 0f;
                protected override float MaxProgress     => 1f;

                private UITemplateQuestController otherQuest;

                protected override void Initialize()
                {
                    this.otherQuest = this.questManager.GetController(this.Condition.QuestId);
                }
            }
        }
    }
}