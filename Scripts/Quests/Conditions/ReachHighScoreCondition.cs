#if THEONE_HIGHSCORE
namespace TheOneStudio.UITemplate.Quests.Conditions
{
    using System;
    using Newtonsoft.Json;
    using TheOneStudio.HighScore;
    using TheOneStudio.HighScore.Models;
    using UnityEngine.Scripting;

    [Preserve]
    public sealed class ReachHighScoreCondition : BaseCondition
    {
        [JsonProperty] private string        Key       { get; [Preserve] set; } = IHighScoreManager.DEFAULT_KEY;
        [JsonProperty] private HighScoreType Type      { get; [Preserve] set; } = HighScoreType.AllTime;
        [JsonProperty] private int           HighScore { get; [Preserve] set; }

        protected override ICondition.IProgress SetupProgress() => new Progress();

        [Preserve]
        private sealed class Progress : BaseProgress
        {
            protected override Type HandlerType => typeof(Handler);

            private sealed class Handler : BaseHandler<ReachHighScoreCondition, Progress>
            {
                private readonly IHighScoreManager highScoreManager;

                [Preserve]
                public Handler(IHighScoreManager highScoreManager)
                {
                    this.highScoreManager = highScoreManager;
                }

                protected override float CurrentProgress => this.highScoreManager.GetHighScore(this.Condition.Key, this.Condition.Type);
                protected override float MaxProgress     => this.Condition.HighScore;
            }
        }
    }
}
#endif