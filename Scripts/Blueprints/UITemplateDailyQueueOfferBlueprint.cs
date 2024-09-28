#if THEONE_DAILY_QUEUE_REWARD
namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using BlueprintFlow.BlueprintReader;
    using UnityEngine.Scripting;

    [Preserve]
    [BlueprintReader("UITemplateDailyQueueOffer", true)]
    public class UITemplateDailyQueueOfferBlueprint : GenericBlueprintReaderByRow<int, UITemplateDailyQueueOfferRecord>
    {
    }

    [Preserve]
    [CsvHeaderKey("Day")]
    public class UITemplateDailyQueueOfferRecord
    {
        public int                                                         Day        { get; [Preserve] private set; }
        public BlueprintByRow<string, UITemplateDailyQueueOfferItemRecord> OfferItems { get; [Preserve] private set; }
    }

    [Preserve]
    [CsvHeaderKey("OfferId")]
    public class UITemplateDailyQueueOfferItemRecord
    {
        public string OfferId       { get; [Preserve] private set; }
        public string ItemId        { get; [Preserve] private set; }
        public string ImageId       { get; [Preserve] private set; }
        public int    Value         { get; [Preserve] private set; }
        public bool   IsRewardedAds { get; [Preserve] private set; }
    }
}
#endif