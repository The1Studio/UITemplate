namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BlueprintFlow.Signals;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using UnityEngine;
    using Zenject;

    public class UITemplateInventoryDataController : IInitializable
    {
        #region inject

        private readonly UITemplateInventoryData     uiTemplateInventoryData;
        private readonly UITemplateCurrencyBlueprint uiTemplateCurrencyBlueprint;
        private readonly UITemplateShopBlueprint     uiTemplateShopBlueprint;
        private readonly SignalBus                   signalBus;
        private readonly UITemplateItemBlueprint     uiTemplateItemBlueprint;

        #endregion

        private const string DefaultSoftCurrencyID = "Coin";

        public UITemplateInventoryDataController(UITemplateInventoryData     uiTemplateInventoryData,
                                                 UITemplateCurrencyBlueprint uiTemplateCurrencyBlueprint,
                                                 UITemplateShopBlueprint     uiTemplateShopBlueprint,
                                                 SignalBus                   signalBus,
                                                 UITemplateItemBlueprint     uiTemplateItemBlueprint)
        {
            this.uiTemplateInventoryData     = uiTemplateInventoryData;
            this.uiTemplateCurrencyBlueprint = uiTemplateCurrencyBlueprint;
            this.uiTemplateShopBlueprint     = uiTemplateShopBlueprint;
            this.signalBus                   = signalBus;
            this.uiTemplateItemBlueprint     = uiTemplateItemBlueprint;

            this.signalBus.Subscribe<LoadBlueprintDataSucceedSignal>(this.OnLoadBlueprintSuccess);
        }

        public string GetCurrentItemSelected(string category)
        {
            return this.uiTemplateInventoryData.CategoryToChosenItem.TryGetValue(category, out var currentId) ? currentId : null;
        }

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

        public UITemplateCurrencyData GetCurrency(string id = DefaultSoftCurrencyID)
        {
            return this.uiTemplateInventoryData.IDToCurrencyData.GetOrAdd(id, () =>
            {
                var currencyRecord = this.uiTemplateCurrencyBlueprint.GetDataById(id);

                return new UITemplateCurrencyData(id, currencyRecord.Max);
            });
        }

        public bool HasItem(string id)
        {
            return this.uiTemplateInventoryData.IDToItemData.ContainsKey(id);
        }

        public bool TryGetItemData(string id, out UITemplateItemData itemData)
        {
            return this.uiTemplateInventoryData.IDToItemData.TryGetValue(id, out itemData);
        }

        public UITemplateItemData GetItemData(string id)
        {
            return this.uiTemplateInventoryData.IDToItemData.GetOrAdd(id, () =>
            {
                var itemRecord = this.uiTemplateShopBlueprint.GetDataById(id);

                return new UITemplateItemData(id, itemRecord);
            });
        }

        public void AddItemData(UITemplateItemData itemData)
        {
            if (this.uiTemplateInventoryData.IDToItemData.TryGetValue(itemData.Id, out var data))
            {
                if (data != itemData)
                {
                    this.uiTemplateInventoryData.IDToItemData.Remove(itemData.Id);
                }
                else return;
            }

            this.uiTemplateInventoryData.IDToItemData.Add(itemData.Id, itemData);
        }

        public void AddCurrency(int addingValue, string id = DefaultSoftCurrencyID)
        {
            this.signalBus.Fire(new UpdateCurrencySignal()
            {
                Id         = id,
                Amount     = addingValue,
                FinalValue = this.uiTemplateInventoryData.IDToCurrencyData[id].Value + addingValue,
            });

            this.uiTemplateInventoryData.IDToCurrencyData[id].Value += addingValue;
        }

        public void UpdateCurrency(int currentCoin, string id = DefaultSoftCurrencyID)
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
            var itemData = this.uiTemplateInventoryData.IDToItemData.GetOrAdd(id, () =>
            {
                var itemRecord = this.uiTemplateShopBlueprint.GetDataById(id);

                return new UITemplateItemData(id, itemRecord, status);
            });
            itemData.CurrentStatus = status;
            return itemData;
        }

        public void Initialize()
        {
        }

        private void OnLoadBlueprintSuccess()
        {
            this.signalBus.Unsubscribe<LoadBlueprintDataSucceedSignal>(this.OnLoadBlueprintSuccess);

            foreach (var item in this.uiTemplateItemBlueprint.Values)
            {
                // Add item to inventory
                // if item exist in shop blueprint, it's status will be unlocked or owned if IsDefaultItem is true
                // else it's status will be locked

                var status = UITemplateItemData.Status.Locked;

                if (this.uiTemplateShopBlueprint.TryGetValue(item.Id, out var shopRecord))
                {
                    status = UITemplateItemData.Status.Unlocked;
                }

                if (item.IsDefaultItem)
                {
                    status = UITemplateItemData.Status.Owned;
                }

                if (!this.uiTemplateInventoryData.IDToItemData.TryGetValue(item.Id, out _))
                {
                    this.uiTemplateInventoryData.IDToItemData.Add(item.Id, new UITemplateItemData(item.Id, shopRecord, status));
                }
            }
        }
    }
}