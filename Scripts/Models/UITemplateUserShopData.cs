namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.Interfaces;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;

    public class UITemplateUserShopData:ILocalData
    {
        #region inject

        private readonly UITemplateShopBlueprint uiTemplateShopBlueprint;

        #endregion

        private Dictionary<string, UITemplateItemData> itemIdToItemData = new();

        public UITemplateUserShopData(UITemplateShopBlueprint uiTemplateShopBlueprint) { this.uiTemplateShopBlueprint = uiTemplateShopBlueprint; }

        public List<UITemplateItemData> GetAllItem(string category = null, UITemplateItemData.UnlockType unlockType = UITemplateItemData.UnlockType.All)
        {
            return this.uiTemplateShopBlueprint.Values.Select(itemRecord => this.GetItemData(itemRecord.Id))
                .Where(itemData => string.IsNullOrEmpty(category) || itemData.BlueprintRecord.Category.Equals(category) && itemData.BlueprintRecord.UnlockType == unlockType)
                .ToList();
        }

        public List<UITemplateItemData> GetAllItemWithOrder(string category = null, UITemplateItemData.UnlockType unlockType = UITemplateItemData.UnlockType.All,
            IComparer<UITemplateItemData> comparer = null)
        {
            return this.GetAllItem(category, unlockType).OrderBy(itemData => itemData, comparer ?? UITemplateItemData.DefaultComparerInstance).ToList();
        }

        public UITemplateItemData GetItemData(string id, UITemplateItemData.Status defaultStatusWhenCreateNew = UITemplateItemData.Status.Locked)
        {
            return this.itemIdToItemData.GetOrAdd(id, () =>
            {
                var itemRecord = this.uiTemplateShopBlueprint.GetDataById(id);

                return new UITemplateItemData(id, itemRecord, defaultStatusWhenCreateNew);
            });
        }

        public UITemplateItemData UpdateStatusItemData(string id, UITemplateItemData.Status status)
        {
            return this.itemIdToItemData.GetOrAdd(id, () =>
            {
                var itemRecord = this.uiTemplateShopBlueprint.GetDataById(id);

                return new UITemplateItemData(id, itemRecord, status);
            });
        }

        public void Init() {  }
    }
}