namespace UITemplate.Scripts.Models
{
    using System.Collections.Generic;

    public class UITemplateInventoryData
    {
        public readonly Dictionary<string, int> CurrencyIdToAmount = new();
        public readonly HashSet<string>         CollectionId       = new();
    }
}