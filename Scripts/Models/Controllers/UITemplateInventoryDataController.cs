namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BlueprintFlow.Signals;
    using GameFoundation.DI;
    using GameFoundation.Signals;
    using TheOne.Extensions;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using UnityEngine;
    using UnityEngine.Scripting;

    public class UITemplateInventoryDataController : IUITemplateControllerData, IInitializable
    {
        public const string DefaultSoftCurrencyID              = "Coin";
        public const string DefaultChestRoomKeyCurrencyID      = "ChestRoomKey";
        public const string DefaultLuckySpinFreeTurnCurrencyID = "LuckySpinFreeTurn";

        [Preserve]
        public UITemplateInventoryDataController(
            UITemplateInventoryData uiTemplateInventoryData,
            UITemplateCurrencyBlueprint uiTemplateCurrencyBlueprint,
            UITemplateShopBlueprint uiTemplateShopBlueprint,
            SignalBus signalBus,
            UITemplateItemBlueprint uiTemplateItemBlueprint
        )
        {
            this.uiTemplateInventoryData     = uiTemplateInventoryData;
            this.uiTemplateCurrencyBlueprint = uiTemplateCurrencyBlueprint;
            this.uiTemplateShopBlueprint     = uiTemplateShopBlueprint;
            this.signalBus                   = signalBus;
            this.uiTemplateItemBlueprint     = uiTemplateItemBlueprint;
        }

        public void Initialize() { this.signalBus.Subscribe<LoadBlueprintDataSucceedSignal>(this.OnLoadBlueprintSuccess); }

        public List<UITemplateItemData> GetDefaultItemByCategory(string category)
        {
            return this.uiTemplateInventoryData.IDToItemData.Values.Where(itemData =>
                itemData.ItemBlueprintRecord.Category == category && itemData.ItemBlueprintRecord.IsDefaultItem
            ).ToList();
        }

        public Dictionary<string, List<UITemplateItemData>> GetDefaultItemWithCategory()
        {
            return this.uiTemplateInventoryData.IDToItemData.Values
                .GroupBy(itemData => itemData.ItemBlueprintRecord.Category)
                .ToDictionary(
                    group => group.Key,
                    group => group.Where(itemData => itemData.ItemBlueprintRecord.IsDefaultItem).ToList()
                );
        }

        public string GetTempCurrencyKey(string currency) { return $"Temp_{currency}"; }

        public void ApplyTempCurrency(string currency) { this.UpdateCurrency(this.GetCurrencyValue(this.GetTempCurrencyKey(currency)), currency); }

        public void ResetTempCurrency(string currency) { this.UpdateCurrency(this.GetCurrencyValue(currency), this.GetTempCurrencyKey(currency)); }

        public string GetCurrentItemSelected(string category) { return this.uiTemplateInventoryData.CategoryToChosenItem.TryGetValue(category, out var currentId) ? currentId : null; }

        public void UpdateCurrentSelectedItem(string category, string id)
        {
            if (this.uiTemplateInventoryData.CategoryToChosenItem.TryGetValue(category, out var currentId))
                this.uiTemplateInventoryData.CategoryToChosenItem[category] = id;
            else
                this.uiTemplateInventoryData.CategoryToChosenItem.Add(category, id);
        }

        public int GetCurrencyValue(string id = DefaultSoftCurrencyID)
        {
            return this.uiTemplateInventoryData.IDToCurrencyData
                .GetOrAdd(id, () => new(id, 0, this.uiTemplateCurrencyBlueprint.GetDataById(id).Max)).Value;
        }

        public UITemplateCurrencyData GetCurrencyData(string id = DefaultSoftCurrencyID)
        {
            return this.uiTemplateInventoryData.IDToCurrencyData.GetOrAdd(id, () => new(id, 0, this.uiTemplateCurrencyBlueprint.GetDataById(id).Max));
        }

        public bool IsCurrencyFull(string id) { return this.GetCurrencyValue(id) >= this.uiTemplateCurrencyBlueprint.GetDataById(id).Max; }

        public bool HasItem(string id) { return this.uiTemplateInventoryData.IDToItemData.ContainsKey(id); }

        public bool TryGetItemData(string id, out UITemplateItemData itemData) { return this.uiTemplateInventoryData.IDToItemData.TryGetValue(id, out itemData); }

        public UITemplateItemData GetItemData(string id, Status defaultStatusWhenCreateNew = Status.Locked)
        {
            var itemRecord = this.uiTemplateItemBlueprint.GetDataById(id);
            var shopRecord = this.uiTemplateShopBlueprint.GetDataById(id);
            var item       = this.uiTemplateInventoryData.IDToItemData.GetOrAdd(id, () => new(id, shopRecord, itemRecord, defaultStatusWhenCreateNew));
            item.ShopBlueprintRecord = shopRecord;
            item.ItemBlueprintRecord = itemRecord;

            return item;
        }

        public void SetOwnedItemData(UITemplateItemData itemData, bool isSelected = false)
        {
            itemData.CurrentStatus = Status.Owned;
            this.AddItemData(itemData);

            if (!isSelected) return;
            this.UpdateCurrentSelectedItem(itemData.ItemBlueprintRecord.Category, itemData.Id);
        }

        public void AddItemData(UITemplateItemData itemData)
        {
            if (this.uiTemplateInventoryData.IDToItemData.TryGetValue(itemData.Id, out var data))
            {
                if (data != itemData)
                    this.uiTemplateInventoryData.IDToItemData.Remove(itemData.Id);
                else
                    return;
            }

            this.uiTemplateInventoryData.IDToItemData.Add(itemData.Id, itemData);
        }

        public void PayCurrency(Dictionary<string, int> currency, string where, int time = 1, Dictionary<string, object> metadata = null)
        {
            foreach (var (currencyKey, currencyValue) in currency) this.AddCurrency(-currencyValue * time, currencyKey, where, metadata: metadata);
        }

        public bool PayCurrency(int value, string id, string where, Dictionary<string, object> metadata = null) => this.AddCurrency(-value, id, where, metadata: metadata);

        /// <summary>
        /// minAnimAmount and maxAnimAmount is range amount of currency object that will be animated
        /// </summary>
        public bool AddCurrency(
            int addingValue,
            string currencyId,
            string placement,
            RectTransform startAnimationRect = null,
            string claimSoundKey = null,
            string flyCompleteSoundKey = null,
            int minAnimAmount = 6,
            int maxAnimAmount = 10,
            float timeAnimAnim = 1f,
            float flyPunchPositionAnimFactor = 0.3f,
            Action onCompleteEachItem = null,
            Dictionary<string, object> metadata = null
        )
        {
            var lastValue = this.GetCurrencyValue(currencyId);

            var resultValue = lastValue + addingValue;
            if (resultValue < 0)
            {
                this.signalBus.Fire(new OnNotEnoughCurrencySignal(currencyId));

                return false;
            }

            var currencyWithCap = this.SetCurrencyWithCap(resultValue, currencyId);
            var amount          = currencyWithCap - lastValue;
            this.signalBus.Fire(new OnUpdateCurrencySignal(currencyId, amount, currencyWithCap, placement, metadata));
            if (startAnimationRect != null)
            {
                this.signalBus.Fire(new PlayCurrencyAnimationSignal
                {
                    currecyId                  = currencyId,
                    amount                     = amount,
                    currencyWithCap            = currencyWithCap,
                    startAnimationRect         = startAnimationRect,
                    claimSoundKey              = claimSoundKey,
                    flyCompleteSoundKey        = flyCompleteSoundKey,
                    minAnimAmount              = minAnimAmount,
                    maxAnimAmount              = maxAnimAmount,
                    timeAnimAnim               = timeAnimAnim,
                    flyPunchPositionAnimFactor = flyPunchPositionAnimFactor,
                    onCompleteEachItem         = onCompleteEachItem
                });
            }
            // if there is no animation, just update the currency
            else
            {
                this.signalBus.Fire(new OnFinishCurrencyAnimationSignal(currencyId, amount, currencyWithCap));
            }

            return true;
        }

        public void UpdateCurrency(int finalValue, string id = DefaultSoftCurrencyID, Dictionary<string, object> metadata = null)
        {
            var lastValue = this.GetCurrencyValue(id);

            var currencyWithCap = this.SetCurrencyWithCap(finalValue, id);
            this.signalBus.Fire(new OnUpdateCurrencySignal(id, currencyWithCap - lastValue, currencyWithCap, metadata: metadata));
        }

        private int SetCurrencyWithCap(int value, string id)
        {
            var uiTemplateCurrencyData = this.uiTemplateInventoryData.IDToCurrencyData[id];
            uiTemplateCurrencyData.Value = Math.Min(uiTemplateCurrencyData.MaxValue, value);

            return uiTemplateCurrencyData.Value;
        }

        public UITemplateItemData GetFirstItem(
            string category = null,
            UnlockType unlockType = UnlockType.All,
            IComparer<UITemplateItemData> orderBy = null,
            params Status[] statuses
        )
        {
            return this.GetAllItems(category, unlockType, orderBy, statuses).FirstOrDefault();
        }

        public List<UITemplateItemData> GetAllItems(
            string category = null,
            UnlockType unlockType = UnlockType.All,
            IComparer<UITemplateItemData> orderBy = null,
            params Status[] statuses
        )
        {
            var query                               = this.uiTemplateInventoryData.IDToItemData.Values.ToList();
            if (category is { }) query              = query.Where(itemData => itemData.ItemBlueprintRecord.Category.Equals(category)).ToList();
            if (unlockType != UnlockType.All) query = query.Where(itemData => (itemData.ShopBlueprintRecord.UnlockType & unlockType) != 0).ToList();
            if (statuses.Length > 0) query          = query.Where(itemData => statuses.Contains(itemData.CurrentStatus)).ToList();
            if (orderBy is { }) query               = query.OrderBy(itemData => itemData, orderBy).ToList();

            return query;
        }

        public List<UITemplateItemData> GetAllItemWithOrder(
            string category = null,
            UnlockType unlockType = UnlockType.All,
            IComparer<UITemplateItemData> comparer = null
        )
        {
            return this.GetAllItems(category, unlockType).OrderBy(itemData => itemData, comparer ?? UITemplateItemData.DefaultComparerInstance).ToList();
        }

        public UITemplateItemData UpdateStatusItemData(string id, Status status)
        {
            var itemData = this.uiTemplateInventoryData.IDToItemData.GetOrAdd(id,
                () =>
                {
                    var shopRecord = this.uiTemplateShopBlueprint.GetDataById(id);
                    var itemRecord = this.uiTemplateItemBlueprint.GetDataById(id);

                    return new(id, shopRecord, itemRecord, status);
                });

            itemData.CurrentStatus = status;

            return itemData;
        }

        private void OnLoadBlueprintSuccess()
        {
            //remove item that don't exist in blueprint anymore
            foreach (var notExistKey in this.uiTemplateInventoryData.IDToItemData.Keys.Where(itemKey => !this.uiTemplateItemBlueprint.ContainsKey(itemKey)).ToList())
                this.uiTemplateInventoryData.IDToItemData.Remove(notExistKey);

            foreach (var itemRecord in this.uiTemplateItemBlueprint.Values)
            {
                // Add item to inventory
                // if item exist in shop blueprint, it's status will be unlocked or owned if IsDefaultItem is true
                // else it's status will be locked

                var status = Status.Locked;

                if (this.uiTemplateShopBlueprint.TryGetValue(itemRecord.Id, out var shopRecord)) status = Status.Unlocked;

                if (itemRecord.IsDefaultItem) status = Status.Owned;

                if (!this.uiTemplateInventoryData.IDToItemData.TryGetValue(itemRecord.Id, out var existedItemData))
                {
#if CREATIVE
                    this.uiTemplateInventoryData.IDToItemData.Add(itemRecord.Id, new UITemplateItemData(itemRecord.Id, shopRecord, itemRecord, Status.Owned));
#else
                    this.uiTemplateInventoryData.IDToItemData.Add(itemRecord.Id, new(itemRecord.Id, shopRecord, itemRecord, status));
#endif
                }
                else
                {
#if CREATIVE
                    existedItemData.CurrentStatus = Status.Owned;
#endif
                    existedItemData.ShopBlueprintRecord = shopRecord;
                    existedItemData.ItemBlueprintRecord = itemRecord;
                }
            }

            // Set default item
            var defaultItemWithCategory = this.GetDefaultItemWithCategory();

            foreach (var (category, defaultItems) in defaultItemWithCategory)
            {
                var currentItemSelected = this.GetCurrentItemSelected(category);

                if (currentItemSelected is { } && this.uiTemplateItemBlueprint.ContainsKey(currentItemSelected)) continue;
                if (defaultItems is null or { Count: 0 }) continue;
                this.UpdateCurrentSelectedItem(category, defaultItems[0].Id);
            }
        }

        /// <summary>
        /// add reward from multiple faucet
        /// </summary>
        /// <param name="rewardKey"></param>
        /// <param name="rewardValue"></param>
        /// <param name="from">where players earn reward</param>
        /// <param name="startPosCurrency"></param>
        /// <param name="claimSoundKey"></param>
        /// <param name="metadata"></param>
        /// <exception cref="Exception"></exception>
        public void AddGenericReward(string rewardKey, int rewardValue, string from, RectTransform startPosCurrency = null, string claimSoundKey = null)
        {
            if (this.uiTemplateCurrencyBlueprint.ContainsKey(rewardKey))
                this.AddCurrency(rewardValue, rewardKey, from, startPosCurrency, claimSoundKey, metadata: new Dictionary<string, object> { { "item_type", rewardKey }, { "item_id", rewardKey }, {"source", from} });
            else if (this.uiTemplateItemBlueprint.ContainsKey(rewardKey))
            {
                this.uiTemplateInventoryData.IDToItemData[rewardKey].CurrentStatus = Status.Owned;
                this.signalBus.Fire(new OnUpdateItemDataSignal(rewardKey, Status.Owned));
            }
            else
                throw new("Need to implemented!!!");
        }

        /// <summary>
        /// add reward from multiple faucet
        /// </summary>
        /// <param name="reward"></param>
        /// <param name="where">where you earn reward from</param>
        /// <param name="startPosCurrency"></param>
        /// <param name="metadata"></param>
        public void AddGenericReward(Dictionary<string, int> reward, string where, RectTransform startPosCurrency = null)
        {
            foreach (var (rewardKey, rewardValue) in reward)
            {
                this.AddGenericReward(rewardKey, rewardValue, where, startPosCurrency);
            }
        }

        public bool IsAlreadyContainedItem(Dictionary<string, int> reward)
        {
            foreach (var (rewardKey, _) in reward)
            {
                if (this.uiTemplateItemBlueprint.TryGetValue(rewardKey, out _))
                    if (this.uiTemplateInventoryData.IDToItemData[rewardKey].CurrentStatus == Status.Owned)
                        return true;
            }

            return false;
        }

        public Dictionary<string, UITemplateItemData> GetAllItemAvailable()
        {
            return this.uiTemplateInventoryData.IDToItemData
                .Where(itemData => itemData.Value.CurrentStatus != Status.Owned && itemData.Value.CurrentStatus == Status.Unlocked)
                .ToDictionary(itemData => itemData.Key, itemData => itemData.Value);
        }

        public bool IsAffordCurrency(Dictionary<string, int> currency, int time = 1)
        {
            foreach (var (currencyKey, currencyValue) in currency)
                if (this.GetCurrencyValue(currencyKey) < currencyValue * time)
                    return false;

            return true;
        }

        public bool IsAffordCurrency(string currencyName, int amount) { return this.GetCurrencyValue(currencyName) >= amount; }

        #region Inject

        private readonly SignalBus                   signalBus;
        private readonly UITemplateInventoryData     uiTemplateInventoryData;
        private readonly UITemplateCurrencyBlueprint uiTemplateCurrencyBlueprint;
        private readonly UITemplateShopBlueprint     uiTemplateShopBlueprint;
        private readonly UITemplateItemBlueprint     uiTemplateItemBlueprint;

        #endregion
    }
}