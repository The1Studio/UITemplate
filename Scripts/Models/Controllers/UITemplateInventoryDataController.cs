namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using Zenject;

    public class UITemplateInventoryDataController : IInitializable
    {
        #region inject

        private readonly UITemplateInventoryData uiTemplateInventoryData;
        private readonly UITemplateCurrencyBlueprint uiTemplateCurrencyBlueprint;
        private readonly UITemplateShopBlueprint     uiTemplateShopBlueprint;
        private readonly SignalBus                   signalBus;

        #endregion

        public UITemplateInventoryDataController(UITemplateInventoryData uiTemplateInventoryData, UITemplateCurrencyBlueprint uiTemplateCurrencyBlueprint, UITemplateShopBlueprint uiTemplateShopBlueprint, SignalBus signalBus)
        {
            this.uiTemplateInventoryData = uiTemplateInventoryData;
            this.uiTemplateCurrencyBlueprint = uiTemplateCurrencyBlueprint;
            this.uiTemplateShopBlueprint     = uiTemplateShopBlueprint;
            this.signalBus                   = signalBus;
        }

        public string GetCurrentItemSelected(string category) { return this.uiTemplateInventoryData.CategoryToChosenItem.TryGetValue(category, out var currentId) ? currentId : null; }

        public void UpdateCurrentSelectedItem(string category, string id)
        {
            if (this.uiTemplateInventoryData.CategoryToChosenItem.TryGetValue(category, out var currentId))
            {
                this.uiTemplateInventoryData.CategoryToChosenItem[category] = id;
            }
            else
            {
                this.uiTemplateInventoryData.CategoryToChosenItem.Add(category, id);
            }
        }

        public UITemplateCurrencyData GetCurrency(string id = "Coin")
        {
            return this.uiTemplateInventoryData.IDToCurrencyData.GetOrAdd(id, () =>
                                                                                  {
                                                                                      var currencyRecord = this.uiTemplateCurrencyBlueprint.GetDataById(id);

                                                                                      return new UITemplateCurrencyData(id, currencyRecord.Max);
                                                                                  });
        }

        public bool HasItem(string id) { return this.uiTemplateInventoryData.IDToItemData.ContainsKey(id); }

        public UITemplateItemData GetItemData(string id)
        {
            return this.uiTemplateInventoryData.IDToItemData.GetOrAdd(id, () =>
                                                                              {
                                                                                  var itemRecord = this.uiTemplateShopBlueprint.GetDataById(id);

                                                                                  return new UITemplateItemData(id, itemRecord);
                                                                              });
        }

        public void AddItemData(UITemplateItemData itemData) { this.uiTemplateInventoryData.IDToItemData.Add(itemData.Id, itemData); }

        public void UpdateCurrency(int currentCoin, string id = "Coin")
        {
            this.signalBus.Fire(new UpdateCurrencySignal()
                                {
                                    Id         = id,
                                    Amount     = currentCoin - this.uiTemplateInventoryData.IDToCurrencyData[id].Value,
                                    FinalValue = currentCoin,
                                });

            this.uiTemplateInventoryData.IDToCurrencyData[id].Value = currentCoin;
        }
        
        public List<UITemplateItemData> GetAllItem(string category = null, UITemplateItemData.UnlockType unlockType = UITemplateItemData.UnlockType.All)
        {
            return this.uiTemplateShopBlueprint.Values.Select(itemRecord => this.GetItemData(itemRecord.Id))
                       .Where(itemData => string.IsNullOrEmpty(category) || itemData.BlueprintRecord.Category.Equals(category) && itemData.BlueprintRecord.UnlockType == unlockType)
                       .ToList();
        }

        public List<UITemplateItemData> GetAllItemWithOrder(string                        category = null, UITemplateItemData.UnlockType unlockType = UITemplateItemData.UnlockType.All,
                                                            IComparer<UITemplateItemData> comparer = null)
        {
            return this.GetAllItem(category, unlockType).OrderBy(itemData => itemData, comparer ?? UITemplateItemData.DefaultComparerInstance).ToList();
        }

        public UITemplateItemData GetItemData(string id, UITemplateItemData.Status defaultStatusWhenCreateNew = UITemplateItemData.Status.Locked)
        {
            return this.uiTemplateInventoryData.IDToItemData.GetOrAdd(id, () =>
                                                                          {
                                                                              var itemRecord = this.uiTemplateShopBlueprint.GetDataById(id);

                                                                              return new UITemplateItemData(id, itemRecord, defaultStatusWhenCreateNew);
                                                                          });
        }

        public UITemplateItemData UpdateStatusItemData(string id, UITemplateItemData.Status status)
        {
            return this.uiTemplateInventoryData.IDToItemData.GetOrAdd(id, () =>
                                                                          {
                                                                              var itemRecord = this.uiTemplateShopBlueprint.GetDataById(id);

                                                                              return new UITemplateItemData(id, itemRecord, status);
                                                                          });
        }

        public void Initialize() { }
    }
}