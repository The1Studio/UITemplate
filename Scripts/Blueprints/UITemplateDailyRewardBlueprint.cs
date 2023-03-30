namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using System.Collections.Generic;
    using BlueprintFlow.BlueprintReader;

    [CsvHeaderKey("Day")]
    [BlueprintReader("UITemplateDailyReward", true)]
    public class UITemplateDailyRewardBlueprint : GenericBlueprintReaderByRow<int, UITemplateDailyRewardRecord>
    {
    }

    public class UITemplateDailyRewardRecord
    {
        public int                     Day         { get; set; }
        public Dictionary<string, int> Reward      { get; set; }
        public string                  RewardImage { get; set; }
    }
}