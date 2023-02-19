namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;

    public class UITemplateItemData
    {
        public static readonly DefaultComparer DefaultComparerInstance = new();

        public string Id;
        public Status CurrentStatus;
        public float  ProgressValue;

        [JsonIgnore] public UITemplateShopRecord BlueprintRecord;

        public UITemplateItemData(string id, UITemplateShopRecord blueprintRecord, Status currentStatus = Status.Locked)
        {
            this.Id              = id;
            this.CurrentStatus   = currentStatus;
            this.BlueprintRecord = blueprintRecord;
        }

        public enum Status
        {
            Owned      = 0,
            InProgress = 1,
            Unlocked   = 2,
            Locked     = 3
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
            All          = IAP | SoftCurrency | Ads | Progression | Gift
        }

        public class DefaultComparer : IComparer<UITemplateItemData>
        {
            public int Compare(UITemplateItemData x, UITemplateItemData y)
            {
                //Check ref and null first
                if (ReferenceEquals(x,    y)) return 0;
                if (ReferenceEquals(null, y)) return 1;
                if (ReferenceEquals(null, x)) return -1;

                //Check status first
                var currentStatusComparison = x.CurrentStatus.CompareTo(y.CurrentStatus);
                if (currentStatusComparison != 0) return currentStatusComparison;

                //If status is equal, then check progress
                var progressComparison = x.ProgressValue.CompareTo(y.ProgressValue);
                if (progressComparison != 0) return progressComparison;

                //if progress is equal, then check id
                return string.Compare(x.Id, y.Id, StringComparison.Ordinal);
            }
        }
    }
}