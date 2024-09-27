#if THEONE_DAILY_QUEUE_REWARD
namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using BlueprintFlow.BlueprintReader;
    using UnityEngine.Scripting;

    [Preserve]
    [BlueprintReader("UITemplateDailyQueueOffer", true)]
    [CsvHeaderKey("Day")]
    public class UITemplateDailyQueueOfferBlueprint : GenericBlueprintReaderByRow<int, UITemplateDailyQueueOfferRecord>
    {
    }

    [Preserve]
    public class UITemplateDailyQueueOfferRecord
    {
        public int                                                         Day       { get; set; }
        public BlueprintByRow<string, UITemplateDailyQueueOfferItemRecord> OfferItems { get; set; }
    }

    [Preserve]
    [CsvHeaderKey("OfferId")]
    public class UITemplateDailyQueueOfferItemRecord
    {
        public string OfferId       { get; set; }
        public string ItemId        { get; set; }
        public string ImageId       { get; set; }
        public int    Value         { get; set; }
        public bool   IsRewardedAds { get; set; }
    }
}
#endif