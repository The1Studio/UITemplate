namespace TheOneStudio.UITemplate.UITemplate.Blueprints.Enums
{
    using System;

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