namespace TheOneStudio.UITemplate.Quests.Conditions
{
    using System;
    using Newtonsoft.Json;
    using TheOneStudio.UITemplate.HighScore;

    public sealed class ReachHighScoreCondition : BaseCondition
    {
        [JsonProperty] private string Key       { get; set; } = UITemplateHighScoreDataController.DEFAULT_KEY;
        [JsonProperty] private int    HighScore { get; set; }

        protected override ICondition.IProgress SetupProgress() => new Progress();

        private sealed class Progress : BaseProgress
        {
            protected override Type HandlerType => typeof(Handler);

            private sealed class Handler : BaseHandler<ReachHighScoreCondition, Progress>
            {
                private readonly UITemplateHighScoreDataController highScoreDataController;

                public Handler(UITemplateHighScoreDataController highScoreDataController)
                {
                    this.highScoreDataController = highScoreDataController;
                }

                protected override float CurrentProgress => this.highScoreDataController.GetAllTimeHighScore(this.Condition.Key);
                protected override float MaxProgress     => this.Condition.HighScore;
            }
        }
    }
}