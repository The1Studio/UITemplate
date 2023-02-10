namespace UITemplate.Scripts.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.Utilities.Extension;
    using Newtonsoft.Json;
    using UITemplate.Scripts.Blueprints;

    public class UITemplateShopData
    {
        #region inject

        private readonly UITemplateShopBlueprint uiTemplateShopBlueprint;

        #endregion

        private Dictionary<string, ItemData> itemIdToItemData = new();

        public UITemplateShopData(UITemplateShopBlueprint uiTemplateShopBlueprint) { this.uiTemplateShopBlueprint = uiTemplateShopBlueprint; }

        public List<ItemData> GetAllItem(string category = null, ItemData.UnlockType unlockType = ItemData.UnlockType.All)
        {
            return this.uiTemplateShopBlueprint.Values.Select(itemRecord => this.GetItemData(itemRecord.Id))
                .Where(itemData => string.IsNullOrEmpty(category) || itemData.BlueprintRecord.Category.Equals(category) && itemData.BlueprintRecord.UnlockType == unlockType)
                .ToList();
        }

        public List<ItemData> GetAllItemWithOrder(string category = null, ItemData.UnlockType unlockType = ItemData.UnlockType.All, IComparer<ItemData> comparer = null)
        {
            return this.GetAllItem(category, unlockType).OrderBy(itemData => itemData, comparer ?? ItemData.DefaultComparerInstance).ToList();
        }

        public ItemData GetItemData(string id)
        {
            return this.itemIdToItemData.GetOrAdd(id, () =>
            {
                var itemRecord = this.uiTemplateShopBlueprint.GetDataById(id);
                return new ItemData(id, itemRecord);
            });
        }
        
        public ItemData UpdateStatusItemData(string id, ItemData.Status status)
        {
            return this.itemIdToItemData.GetOrAdd(id, () =>
            {
                var itemRecord = this.uiTemplateShopBlueprint.GetDataById(id);
                return new ItemData(id, itemRecord, status);
            });
        }
    }

    public class ItemData
    {
        public static readonly DefaultComparer DefaultComparerInstance = new();

        public string Id;
        public Status CurrentStatus;
        public float  ProgressValue;

        [JsonIgnore] public UITemplateShopRecord BlueprintRecord;

        public ItemData(string id, UITemplateShopRecord blueprintRecord, Status currentStatus = Status.Locked)
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

        public class DefaultComparer : IComparer<ItemData>
        {
            public int Compare(ItemData x, ItemData y)
            {
                //Check ref and null first
                if (ReferenceEquals(x, y)) return 0;
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