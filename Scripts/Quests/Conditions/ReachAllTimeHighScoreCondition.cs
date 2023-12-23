namespace TheOneStudio.UITemplate.Quests.Conditions
{
    using System;
    using Newtonsoft.Json;
    using TheOneStudio.UITemplate.UITemplate.Leaderboard;

    public sealed class ReachHighScoreCondition : BaseCondition
    {
        [JsonProperty] private string Key       { get; } = UITemplateLeaderBoardDataController.DEFAULT_KEY;
        [JsonProperty] private int    HighScore { get; }

        protected override ICondition.IProgress SetupProgress() => new Progress();

        private sealed class Progress : BaseProgress
        {
            protected override Type HandlerType => typeof(Handler);

            private sealed class Handler : BaseHandler<ReachHighScoreCondition, Progress>
            {
                private readonly UITemplateLeaderBoardDataController leaderBoardDataController;

                public Handler(UITemplateLeaderBoardDataController leaderBoardDataController)
                {
                    this.leaderBoardDataController = leaderBoardDataController;
                }

                protected override float CurrentProgress => this.leaderBoardDataController.GetAllTimeHighScore(this.Condition.Key);
                protected override float MaxProgress     => this.Condition.HighScore;
            }
        }
    }
}