namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System.Collections.Generic;

    public class UITemplateInventoryData
    {
        public readonly Dictionary<string, int>                CurrencyIdToAmount    = new();
        public readonly HashSet<string>                        CollectionId          = new();
        public readonly Dictionary<string, string>             CategoryToCurrentItem = new();
        public readonly Dictionary<string, UITemplateItemData> IdToItemData          = new();
    }
}