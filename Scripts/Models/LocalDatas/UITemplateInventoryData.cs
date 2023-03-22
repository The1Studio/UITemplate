namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;

    public class UITemplateInventoryData : ILocalData
    {
        public readonly Dictionary<string, string>                 CategoryToChosenItem = new();
        public          Dictionary<string, UITemplateItemData>     IDToItemData     { get; private set; } = new();
        public          Dictionary<string, UITemplateCurrencyData> IDToCurrencyData { get; private set; } = new();

        public void Init() { }
    }
}