namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;

    public class UITemplateItemData
    {
        public static readonly DefaultComparer DefaultComparerInstance = new();

        public readonly string Id;
        public          Status CurrentStatus;
        public          int    RemainingAdsProgress;

        [JsonIgnore] public UITemplateShopRecord ShopBlueprintRecord { get; internal set; }

        [JsonIgnore] public UITemplateItemRecord ItemBlueprintRecord { get; internal set; }

        public UITemplateItemData(
            string               id,
            UITemplateShopRecord shopBlueprintRecord,
            UITemplateItemRecord itemBlueprintRecord,
            Status               currentStatus = Status.Locked
        )
        {
            this.Id                  = id;
            this.CurrentStatus       = currentStatus;
            this.ShopBlueprintRecord = shopBlueprintRecord;
            this.ItemBlueprintRecord = itemBlueprintRecord;

            if (this.ShopBlueprintRecord is { } && (this.ShopBlueprintRecord.UnlockType & UnlockType.Ads) != 0) this.RemainingAdsProgress = this.ShopBlueprintRecord?.Price ?? 0;
        }

        public enum Status
        {
            Owned      = 0,
            Unlocked   = 1,
            InProgress = 2,
            Locked     = 3,
        }

        public class DefaultComparer : IComparer<UITemplateItemData>
        {
            public int Compare(UITemplateItemData x, UITemplateItemData y)
            {
                //Check ref and null first
                if (ReferenceEquals(x, y)) return 0;
                if (ReferenceEquals(null, y)) return 1;
                if (ReferenceEquals(null, x)) return -1;

                //Check status first
                var currentStatusComparison = x.CurrentStatus.CompareTo(y.CurrentStatus);

                if (currentStatusComparison != 0) return currentStatusComparison;

                //If status is equal, then check progress
                var progressComparison = x.RemainingAdsProgress.CompareTo(y.RemainingAdsProgress);

                if (progressComparison != 0) return progressComparison;

                //if progress is equal, then check id
                return string.Compare(x.Id, y.Id, StringComparison.Ordinal);
            }
        }
    }
}