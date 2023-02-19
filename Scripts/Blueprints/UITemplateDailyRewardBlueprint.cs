namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using System.Collections.Generic;
    using BlueprintFlow.BlueprintReader;

    [CsvHeaderKey("Day")]
    [BlueprintReader("UITemplateDailyReward", true)]
    public class UITemplateDailyRewardBlueprint : GenericBlueprintReaderByRow<string, UITemplateDailyRewardRecord>
    {
        
    }

    public class UITemplateDailyRewardRecord
    {
        public int    Day      { get; set; }
        public List<Dictionary<string, int>> Reward { get; set; }
        public string RewardImage { get; set; }
    }
}