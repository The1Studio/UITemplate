namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using BlueprintFlow.BlueprintReader;
    using UnityEngine;

    [CsvHeaderKey("Day")]
    [BlueprintReader("UITemplateDailyReward", true)]
    public class UITemplateDailyRewardBlueprint : GenericBlueprintReaderByRow<int, UITemplateDailyRewardRecord>
    {
    }

    public class UITemplateDailyRewardRecord
    {
        public int                                            Day       { get; set; }
        public string                                         PackImage { get; set; }
        public BlueprintByRow<string, UITemplateRewardRecord> Reward    { get; set; }
    }

    [CsvHeaderKey("RewardId")]
    public class UITemplateRewardRecord
    {
        public string   RewardId    { get; set; }
        public int      RewardValue { get; set; }
        public string   RewardImage { get; set; }
        public Vector2? Position    { get; set; }
        public Vector2? Size        { get; set; }
        public bool     SpoilReward { get; set; }
        public bool     ShowValue   { get; set; }
    }
}