namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;

    public class UITemplateInventoryData : ILocalData
    {
        internal Dictionary<string, string>                 CategoryToChosenItem { get; set; } = new();
        internal Dictionary<string, UITemplateItemData>     IDToItemData         { get; set; } = new();
        internal Dictionary<string, UITemplateCurrencyData> IDToCurrencyData     { get; set; } = new();

        public void Init() { }
    }
}