namespace TheOneStudio.UITemplate.Quests.Conditions
{
    using System;
    using Newtonsoft.Json;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;

    public sealed class WatchAdCountCondition : BaseCondition
    {
        [JsonProperty] private int Count { get; set; }

        protected override ICondition.IProgress SetupProgress() => new Progress();

        private sealed class Progress : BaseProgress
        {
            [JsonProperty] private int? StartCount { get; set; }

            protected override Type HandlerType => typeof(Handler);

            private sealed class Handler : BaseHandler<WatchAdCountCondition, Progress>
            {
                private readonly UITemplateAdsController adsDataController;

                public Handler(UITemplateAdsController adsDataController)
                {
                    this.adsDataController = adsDataController;
                }

                protected override float CurrentProgress => this.adsDataController.WatchRewardedAds - this.Progress.StartCount!.Value;
                protected override float MaxProgress     => this.Condition.Count;

                protected override void Initialize()
                {
                    this.Progress.StartCount ??= this.adsDataController.WatchRewardedAds;
                }
            }
        }
    }
}