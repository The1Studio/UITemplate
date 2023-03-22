namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;

    public class UITemplateInventoryData : ILocalData
    {
        public Dictionary<string, string>                 CategoryToChosenItem { get; set; } = new();
        public Dictionary<string, UITemplateItemData>     IDToItemData         { get; set; } = new();
        public Dictionary<string, UITemplateCurrencyData> IDToCurrencyData     { get; set; } = new();

        public void Init() { }
    }
}