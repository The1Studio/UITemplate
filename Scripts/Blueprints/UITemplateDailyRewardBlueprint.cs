namespace UITemplate.Scripts.Blueprints
{
    using BlueprintFlow.BlueprintReader;

    [CsvHeaderKey("Day")]
    [BlueprintReader("UITemplateDailyReward", true)]
    public class UITemplateDailyRewardBlueprint : GenericBlueprintReaderByRow<string, UITemplateDailyRewardRecord>
    {
        
    }

    public class UITemplateDailyRewardRecord
    {
        public int    Day      { get; set; }
        public string RewardId { get; set; }
        public int    Amount   { get; set; }
    }
}