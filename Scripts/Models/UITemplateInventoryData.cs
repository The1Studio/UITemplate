namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System;
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using Zenject;

    public class UITemplateUserInventoryData : ILocalData
    {
        private readonly UITemplateCurrencyBlueprint uiTemplateCurrencyBlueprint;
        private readonly SignalBus                   signalBus;
        private readonly UITemplateShopBlueprint     uiTemplateShopBlueprint;

        public readonly Dictionary<string, string>                 CategoryToChosenItem = new();
        public          Dictionary<string, UITemplateItemData>     IDToItemData     { get; private set; } = new();
        public          Dictionary<string, UITemplateCurrencyData> IDToCurrencyData { get; private set; } = new();

        public UITemplateUserInventoryData(UITemplateCurrencyBlueprint uiTemplateCurrencyBlueprint, SignalBus signalBus, UITemplateShopBlueprint uiTemplateShopBlueprint)
        {
            this.uiTemplateCurrencyBlueprint = uiTemplateCurrencyBlueprint;
            this.signalBus                   = signalBus;
            this.uiTemplateShopBlueprint     = uiTemplateShopBlueprint;
        }

        public string GetCurrentItemSelected(string category) { return this.CategoryToChosenItem.TryGetValue(category, out var currentId) ? currentId : null; }

        public void UpdateCurrentSelectedItem(string category, string id)
        {
            if (this.CategoryToChosenItem.TryGetValue(category, out var currentId))
            {
                this.CategoryToChosenItem[category] = id;
            }
            else
            {
                this.CategoryToChosenItem.Add(category, id);
            }
        }

        public UITemplateCurrencyData GetCurrency(string id)
        {
            return this.IDToCurrencyData.GetOrAdd(id, () =>
            {
                var currencyRecord = this.uiTemplateCurrencyBlueprint.GetDataById(id);

                return new UITemplateCurrencyData(id, currencyRecord.Max);
            });
        }

        public bool HasItem(string id) { return this.IDToItemData.ContainsKey(id); }

        public UITemplateItemData GetItemData(string id)
        {
            return this.IDToItemData.GetOrAdd(id, () =>
            {
                var itemRecord = this.uiTemplateShopBlueprint.GetDataById(id);

                return new UITemplateItemData(id, itemRecord);
            });
        }

        public void AddItemData(UITemplateItemData itemData) { this.IDToItemData.Add(itemData.Id, itemData); }

        public void UpdateCurrency(string id, UITemplateCurrencyData currentCoin)
        {
            this.signalBus.Fire(new UpdateCurrencySignal()
            {
                Id         = id,
                Amount     = Math.Abs(this.IDToCurrencyData[id].Value - currentCoin.Value),
                FinalValue = currentCoin.Value,
            });

            this.IDToCurrencyData[id] = currentCoin;
        }

        public void Init() { }
    }
}