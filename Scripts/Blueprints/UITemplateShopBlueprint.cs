namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using System;
    using BlueprintFlow.BlueprintReader;
    using UnityEngine.Scripting;

    [Preserve]
    [BlueprintReader("UITemplateShop", true)]
    public class UITemplateShopBlueprint : GenericBlueprintReaderByRow<string, UITemplateShopRecord>
    {
    }

    [Preserve]
    [CsvHeaderKey("Id")]
    public class UITemplateShopRecord
    {
        public string     Id         { get; [Preserve] private set; }
        public UnlockType UnlockType { get; [Preserve] private set; }
        public string     CurrencyID { get; [Preserve] private set; }
        public int        Price      { get; [Preserve] private set; }
    }

    [Flags]
    public enum UnlockType
    {
        None         = 0,
        IAP          = 1 << 1,
        SoftCurrency = 1 << 2,
        Ads          = 1 << 3,
        Progression  = 1 << 4,
        Gift         = 1 << 5,
        DailyReward  = 1 << 6,
        LuckySpin    = 1 << 7,
        StartedPack  = 1 << 8,
        Shard        = 1 << 9,
        All          = -1,
        Default      = IAP | SoftCurrency | Ads | Progression | Gift | StartedPack,
    }
}