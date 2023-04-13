namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BlueprintFlow.Signals;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using UnityEngine;
    using Zenject;

    public class UITemplateInventoryDataController
    {
        #region Inject

        private readonly UITemplateInventoryData           uiTemplateInventoryData;
        private readonly UITemplateFlyingAnimationCurrency uiTemplateFlyingAnimationCurrency;
        private readonly UITemplateCurrencyBlueprint       uiTemplateCurrencyBlueprint;
        private readonly UITemplateShopBlueprint           uiTemplateShopBlueprint;
        private readonly SignalBus                         signalBus;
        private readonly UITemplateItemBlueprint           uiTemplateItemBlueprint;

        #endregion

        public const string DefaultSoftCurrencyID         = "Coin";
        public const string DefaultChestRoomKeyCurrencyID = "ChestRoomKey";

        private Dictionary<string, int> revertCurrencyValue = new();

        public UITemplateInventoryDataController(UITemplateInventoryData uiTemplateInventoryData, UITemplateFlyingAnimationCurrency uiTemplateFlyingAnimationCurrency,
            UITemplateCurrencyBlueprint uiTemplateCurrencyBlueprint, UITemplateShopBlueprint uiTemplateShopBlueprint, SignalBus signalBus,
            UITemplateItemBlueprint uiTemplateItemBlueprint)
        {
            this.uiTemplateInventoryData           = uiTemplateInventoryData;
            this.uiTemplateFlyingAnimationCurrency = uiTemplateFlyingAnimationCurrency;
            this.uiTemplateCurrencyBlueprint       = uiTemplateCurrencyBlueprint;
            this.uiTemplateShopBlueprint           = uiTemplateShopBlueprint;
            this.signalBus                         = signalBus;
            this.uiTemplateItemBlueprint           = uiTemplateItemBlueprint;

            this.signalBus.Subscribe<LoadBlueprintDataSucceedSignal>(this.OnLoadBlueprintSuccess);
        }

        public void SetRevertCurrencyValue(string id, int value)
        {
            if (this.revertCurrencyValue.ContainsKey(id))
            {
                this.revertCurrencyValue[id] = value;
            }
            else
            {
                this.revertCurrencyValue.Add(id, value);
            }
        }

        public void RevertCurrency()
        {
            foreach (var (currencyKey, value) in this.revertCurrencyValue)
            {
                this.UpdateCurrency(value, currencyKey);
            }

            this.revertCurrencyValue.Clear();
        }

        public string GetCurrentItemSelected(string category) => this.uiTemplateInventoryData.CategoryToChosenItem.TryGetValue(category, out var currentId) ? currentId : null;

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

        public int GetCurrencyValue(string id = DefaultSoftCurrencyID)
        {
            return this.uiTemplateInventoryData.IDToCurrencyData.GetOrAdd(id, () => new UITemplateCurrencyData(id, 0, this.uiTemplateCurrencyBlueprint.GetDataById(id).Max)).Value;
        }

        public bool IsCurrencyFull(string id) { return this.GetCurrencyValue(id) >= this.uiTemplateCurrencyBlueprint.GetDataById(id).Max; }

        public bool HasItem(string id) => this.uiTemplateInventoryData.IDToItemData.ContainsKey(id);

        public bool TryGetItemData(string id, out UITemplateItemData itemData) => this.uiTemplateInventoryData.IDToItemData.TryGetValue(id, out itemData);

        public UITemplateItemData GetItemData(string id, UITemplateItemData.Status defaultStatusWhenCreateNew = UITemplateItemData.Status.Locked)
        {
            var itemRecord = this.uiTemplateItemBlueprint.GetDataById(id);
            var shopRecord = this.uiTemplateShopBlueprint.GetDataById(id);
            var item       = this.uiTemplateInventoryData.IDToItemData.GetOrAdd(id, () => new UITemplateItemData(id, shopRecord, itemRecord, defaultStatusWhenCreateNew));
            item.ShopBlueprintRecord = shopRecord;
            item.ItemBlueprintRecord = itemRecord;

            return item;
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

        public async UniTask AddCurrency(int addingValue, string id = DefaultSoftCurrencyID, RectTransform startAnimationRect = null)
        {
            if (startAnimationRect != null)
            {
                await this.uiTemplateFlyingAnimationCurrency.PlayAnimation(startAnimationRect);
            }

            this.signalBus.Fire(new UpdateCurrencySignal() { Id = id, Amount = addingValue, FinalValue = this.GetCurrencyValue(id) + addingValue, });

            this.SetCurrencyWithCap(this.GetCurrencyValue(id) + addingValue, id);
        }

        public void UpdateCurrency(int finalValue, string id = DefaultSoftCurrencyID)
        {
            this.signalBus.Fire(new UpdateCurrencySignal() { Id = id, Amount = finalValue - this.GetCurrencyValue(id), FinalValue = finalValue, });

            this.SetCurrencyWithCap(finalValue, id);
        }

        private void SetCurrencyWithCap(int value, string id)
        {
            var uiTemplateCurrencyData = this.uiTemplateInventoryData.IDToCurrencyData[id];
            uiTemplateCurrencyData.Value = Math.Min(uiTemplateCurrencyData.MaxValue, value);
        }

        public UITemplateItemData FindOneItem(string category = null, UITemplateItemData.UnlockType unlockType = UITemplateItemData.UnlockType.All, IComparer<UITemplateItemData> orderBy = null,
            params UITemplateItemData.Status[] statuses)
        {
            return this.FindAllItems(category, unlockType, orderBy, statuses).FirstOrDefault();
        }

        public List<UITemplateItemData> FindAllItems(string category = null, UITemplateItemData.UnlockType unlockType = UITemplateItemData.UnlockType.All, IComparer<UITemplateItemData> orderBy = null,
            params UITemplateItemData.Status[] statuses)
        {
            var query                                                  = this.uiTemplateInventoryData.IDToItemData.Values.ToList();
            if (category is not null) query                            = query.Where(itemData => itemData.ItemBlueprintRecord.Category.Equals(category)).ToList();
            if (unlockType != UITemplateItemData.UnlockType.All) query = query.Where(itemData => (itemData.ShopBlueprintRecord.UnlockType & unlockType) != 0).ToList();
            if (statuses.Length > 0) query                             = query.Where(itemData => statuses.Contains(itemData.CurrentStatus)).ToList();
            if (orderBy is not null) query                             = query.OrderBy(itemData => itemData, orderBy).ToList();

            return query;
        }

        public List<UITemplateItemData> GetAllItem(string category = null, UITemplateItemData.UnlockType unlockType = UITemplateItemData.UnlockType.All, IComparer<UITemplateItemData> orderBy = null,
            params UITemplateItemData.Status[] statuses)
        {
            return this.FindAllItems(category, unlockType, orderBy, statuses);
        }

        public List<UITemplateItemData> GetAllItemWithOrder(string category = null, UITemplateItemData.UnlockType unlockType = UITemplateItemData.UnlockType.All,
            IComparer<UITemplateItemData> comparer = null)
        {
            return this.GetAllItem(category, unlockType).OrderBy(itemData => itemData, comparer ?? UITemplateItemData.DefaultComparerInstance).ToList();
        }

        public UITemplateItemData UpdateStatusItemData(string id, UITemplateItemData.Status status)
        {
            var itemData = this.uiTemplateInventoryData.IDToItemData.GetOrAdd(id, () =>
            {
                var shopRecord = this.uiTemplateShopBlueprint.GetDataById(id);
                var itemRecord = this.uiTemplateItemBlueprint.GetDataById(id);

                return new UITemplateItemData(id, shopRecord, itemRecord, status);
            });

            itemData.CurrentStatus = status;

            return itemData;
        }

        private void OnLoadBlueprintSuccess()
        {
            this.signalBus.Unsubscribe<LoadBlueprintDataSucceedSignal>(this.OnLoadBlueprintSuccess);

            foreach (var itemRecord in this.uiTemplateItemBlueprint.Values)
            {
                // Add item to inventory
                // if item exist in shop blueprint, it's status will be unlocked or owned if IsDefaultItem is true
                // else it's status will be locked

                var status = UITemplateItemData.Status.Locked;

                if (this.uiTemplateShopBlueprint.TryGetValue(itemRecord.Id, out var shopRecord))
                {
                    status = UITemplateItemData.Status.Unlocked;
                }

                if (itemRecord.IsDefaultItem)
                {
                    status = UITemplateItemData.Status.Owned;
                }

                if (!this.uiTemplateInventoryData.IDToItemData.TryGetValue(itemRecord.Id, out var existedItemData))
                {
#if CREATIVE
                    this.uiTemplateInventoryData.IDToItemData.Add(itemRecord.Id, new UITemplateItemData(itemRecord.Id, shopRecord, itemRecord, UITemplateItemData.Status.Owned));
#else
                    this.uiTemplateInventoryData.IDToItemData.Add(itemRecord.Id, new UITemplateItemData(itemRecord.Id, shopRecord, itemRecord, status));
#endif
                }
                else
                {
#if CREATIVE
                    existedItemData.CurrentStatus = UITemplateItemData.Status.Owned;
#endif
                    existedItemData.ShopBlueprintRecord = shopRecord;
                    existedItemData.ItemBlueprintRecord = itemRecord;
                }
            }
        }

        public void AddGenericReward(string rewardKey, int rewardValue, RectTransform startPosCurrency = null)
        {
            if (this.uiTemplateCurrencyBlueprint.TryGetValue(rewardKey, out _))
            {
                this.AddCurrency(rewardValue, rewardKey, startPosCurrency);
            }
            else if (this.uiTemplateItemBlueprint.TryGetValue(rewardKey, out _))
            {
                this.uiTemplateInventoryData.IDToItemData[rewardKey].CurrentStatus = UITemplateItemData.Status.Owned;
            }
            else
            {
                throw new Exception("Need to implemented!!!");
            }
        }

        public void AddGenericReward(Dictionary<string, int> reward, RectTransform startPosCurrency = null)
        {
            foreach (var (rewardKey, rewardValue) in reward)
            {
                this.AddGenericReward(rewardKey, rewardValue, startPosCurrency);
            }
        }

        public bool IsAlreadyContainedItem(Dictionary<string, int> reward)
        {
            foreach (var (rewardKey, _) in reward)
            {
                if (this.uiTemplateItemBlueprint.TryGetValue(rewardKey, out _))
                {
                    if (this.uiTemplateInventoryData.IDToItemData[rewardKey].CurrentStatus == UITemplateItemData.Status.Owned)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public Dictionary<string, UITemplateItemData> GetAllItemAvailable()
        {
            return this.uiTemplateInventoryData.IDToItemData
                .Where(itemData => itemData.Value.CurrentStatus != UITemplateItemData.Status.Owned && itemData.Value.CurrentStatus == UITemplateItemData.Status.Unlocked)
                .ToDictionary(itemData => itemData.Key, itemData => itemData.Value);
        }
    }
}