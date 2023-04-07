namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;
    using Sirenix.Serialization;

    public class UITemplateInventoryData : ILocalData
    {
        [OdinSerialize]
        public readonly Dictionary<string, string> CategoryToChosenItem = new();

        [OdinSerialize]
        public Dictionary<string, UITemplateItemData> IDToItemData { get; private set; } = new();

        [OdinSerialize]
        public Dictionary<string, UITemplateCurrencyData> IDToCurrencyData { get; private set; } = new();

        public void Init()
        {
        }
    }
}