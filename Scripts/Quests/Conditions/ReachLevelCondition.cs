namespace TheOneStudio.UITemplate.Quests.Conditions
{
    using System;
    using Newtonsoft.Json;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine.Scripting;

    [Preserve]
    public sealed class ReachLevelCondition : BaseCondition
    {
        [JsonProperty] private int Level { get; [Preserve] set; }

        protected override ICondition.IProgress SetupProgress() => new Progress();

        [Preserve]
        private sealed class Progress : BaseProgress
        {
            protected override Type HandlerType => typeof(Handler);

            private sealed class Handler : BaseHandler<ReachLevelCondition, Progress>
            {
                private readonly UITemplateLevelDataController levelDataController;

                [Preserve]
                public Handler(UITemplateLevelDataController levelDataController)
                {
                    this.levelDataController = levelDataController;
                }

                protected override float CurrentProgress => this.levelDataController.CurrentLevel < this.Condition.Level ? 0f : 1f;
                protected override float MaxProgress     => 1;
            }
        }
    }
}