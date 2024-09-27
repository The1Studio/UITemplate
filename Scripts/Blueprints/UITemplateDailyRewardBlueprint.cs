namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using BlueprintFlow.BlueprintReader;
    using UnityEngine;
    using UnityEngine.Scripting;

    [Preserve]
    [BlueprintReader("UITemplateDailyReward", true)]
    public class UITemplateDailyRewardBlueprint : GenericBlueprintReaderByRow<int, UITemplateDailyRewardRecord>
    {
    }

    [Preserve]
    [CsvHeaderKey("Day")]
    public class UITemplateDailyRewardRecord
    {
        public int                                            Day       { get; [Preserve] private set; }
        public string                                         PackImage { get; [Preserve] private set; }
        public BlueprintByRow<string, UITemplateRewardRecord> Reward    { get; [Preserve] private set; }
    }

    [Preserve]
    [CsvHeaderKey("RewardId")]
    public class UITemplateRewardRecord
    {
        public string   RewardId    { get; [Preserve] private set; }
        public int      RewardValue { get; [Preserve] private set; }
        public string   RewardImage { get; [Preserve] private set; }
        public Vector2? Position    { get; [Preserve] private set; }
        public Vector2? Size        { get; [Preserve] private set; }
        public bool     SpoilReward { get; [Preserve] private set; }
        public bool     ShowValue   { get; [Preserve] private set; }
    }
}