namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using System.Collections.Generic;
    using BlueprintFlow.BlueprintReader;
    using UnityEngine;

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
        public Vector2?                Position    { get; set; }
        public Vector2?                Size        { get; set; }
        public bool                    SpoilReward { get; set; }
    }
}