namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BlueprintFlow.Signals;
    using Cysharp.Threading.Tasks;
    using GameFoundation.DI;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.Utilities;
    using GameFoundation.Scripts.Utilities.Extension;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using UnityEngine;
    using UnityEngine.Scripting;

    public class UITemplateInventoryDataController : IUITemplateControllerData, IInitializable
    {
        #region Inject

        private readonly SignalBus                           signalBus;
        private readonly IScreenManager                      screenManager;
        private readonly UITemplateInventoryData             uiTemplateInventoryData;
        private readonly UITemplateFlyingAnimationController uiTemplateFlyingAnimationController;
        private readonly UITemplateCurrencyBlueprint         uiTemplateCurrencyBlueprint;
        private readonly UITemplateShopBlueprint             uiTemplateShopBlueprint;
        private readonly UITemplateItemBlueprint             uiTemplateItemBlueprint;
        private readonly IAudioService                       audioService;

        #endregion

        public const string DefaultSoftCurrencyID              = "Coin";
        public const string DefaultChestRoomKeyCurrencyID      = "ChestRoomKey";
        public const string DefaultLuckySpinFreeTurnCurrencyID = "LuckySpinFreeTurn";

        [Preserve]
        public UITemplateInventoryDataController(UITemplateInventoryData uiTemplateInventoryData, UITemplateFlyingAnimationController uiTemplateFlyingAnimationController,
            UITemplateCurrencyBlueprint uiTemplateCurrencyBlueprint, UITemplateShopBlueprint uiTemplateShopBlueprint, SignalBus signalBus,
            UITemplateItemBlueprint uiTemplateItemBlueprint, IScreenManager screenManager, IAudioService audioService)
        {
            this.uiTemplateInventoryData             = uiTemplateInventoryData;
            this.uiTemplateFlyingAnimationController = uiTemplateFlyingAnimationController;
            this.uiTemplateCurrencyBlueprint         = uiTemplateCurrencyBlueprint;
            this.uiTemplateShopBlueprint             = uiTemplateShopBlueprint;
            this.signalBus                           = signalBus;
            this.uiTemplateItemBlueprint             = uiTemplateItemBlueprint;
            this.screenManager                       = screenManager;
            this.audioService                        = audioService;
        }

        public List<UITemplateItemData> GetDefaultItemByCategory(string category)
        {
            return this.uiTemplateInventoryData.IDToItemData.Values.Where(itemData =>
                itemData.ItemBlueprintRecord.Category == category &&
                itemData.ItemBlueprintRecord.IsDefaultItem
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

        public string GetTempCurrencyKey(string currency) => $"Temp_{currency}";

        public void ApplyTempCurrency(string currency) { this.UpdateCurrency(this.GetCurrencyValue(this.GetTempCurrencyKey(currency)), currency); }

        public void ResetTempCurrency(string currency) { this.UpdateCurrency(this.GetCurrencyValue(currency), this.GetTempCurrencyKey(currency)); }

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

        public int GetCurrencyValue(string id = DefaultSoftCurrencyID) => this.uiTemplateInventoryData.IDToCurrencyData
                                                                              .GetOrAdd(id, () => new UITemplateCurrencyData(id, 0, this.uiTemplateCurrencyBlueprint.GetDataById(id).Max)).Value;

        public UITemplateCurrencyData GetCurrencyData(string id = DefaultSoftCurrencyID) =>
            this.uiTemplateInventoryData.IDToCurrencyData.GetOrAdd(id, () => new UITemplateCurrencyData(id, 0, this.uiTemplateCurrencyBlueprint.GetDataById(id).Max));

        public bool IsCurrencyFull(string id) => this.GetCurrencyValue(id) >= this.uiTemplateCurrencyBlueprint.GetDataById(id).Max;

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

        public void SetOwnedItemData(UITemplateItemData itemData, bool isSelected = false)
        {
            itemData.CurrentStatus = UITemplateItemData.Status.Owned;
            this.AddItemData(itemData);

            if (!isSelected) return;
            this.UpdateCurrentSelectedItem(itemData.ItemBlueprintRecord.Category, itemData.Id);
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

        public void PayCurrency(Dictionary<string, int> currency, int time = 1)
        {
            foreach (var (currencyKey, currencyValue) in currency)
            {
                this.AddCurrency(-currencyValue * time, currencyKey).Forget();
            }
        }

        /// <summary>
        /// minAnimAmount and maxAnimAmount is range amount of currency object that will be animated
        /// </summary>
        public async UniTask<bool> AddCurrency(int addingValue, string id = DefaultSoftCurrencyID, RectTransform startAnimationRect = null, string claimSoundKey = null,
            int minAnimAmount = 6, int maxAnimAmount = 10, float timeAnimAnim = 1f, float flyPunchPositionAnimFactor = 0.3f)
        {
            var lastValue = this.GetCurrencyValue(id);

            var resultValue = lastValue + addingValue;
            if (resultValue < 0)
            {
                this.signalBus.Fire(new OnNotEnoughCurrencySignal(id));
                return false;
            }

            var currencyWithCap = this.SetCurrencyWithCap(resultValue, id);
            var amount = currencyWithCap - lastValue;
            this.signalBus.Fire(new OnUpdateCurrencySignal(id, amount, currencyWithCap));

            if (startAnimationRect != null)
            {
                var flyingObject = this.uiTemplateCurrencyBlueprint.GetDataById(id).FlyingObject;
                var currencyView = this.screenManager.RootUICanvas.GetComponentsInChildren<UITemplateCurrencyView>().FirstOrDefault(viewTarget => viewTarget.CurrencyKey.Equals(id));
                if (currencyView != null)
                {
                    if (!string.IsNullOrEmpty(claimSoundKey)) this.audioService.PlaySound(claimSoundKey);
                    await this.uiTemplateFlyingAnimationController.PlayAnimation<UITemplateCurrencyView>(startAnimationRect,
                        minAnimAmount,
                        maxAnimAmount,
                        timeAnimAnim,
                        currencyView.CurrencyIcon.transform as RectTransform,
                        flyingObject,
                        flyPunchPositionAnimFactor);

                    lastValue = this.GetCurrencyValue(id); // get last value after animation because it can be changed by other animation
                    this.signalBus.Fire(new OnFinishCurrencyAnimationSignal(id, amount, currencyWithCap));
                }
            }
            else
            {
                // if there is no animation, just update the currency
                this.signalBus.Fire(new OnFinishCurrencyAnimationSignal(id, amount, currencyWithCap));
            }

            return true;
        }

        public void UpdateCurrency(int finalValue, string id = DefaultSoftCurrencyID)
        {
            var lastValue = this.GetCurrencyValue(id);

            var currencyWithCap = this.SetCurrencyWithCap(finalValue, id);
            this.signalBus.Fire(new OnUpdateCurrencySignal(id, currencyWithCap - lastValue, currencyWithCap));
        }

        private int SetCurrencyWithCap(int value, string id)
        {
            var uiTemplateCurrencyData = this.uiTemplateInventoryData.IDToCurrencyData[id];
            uiTemplateCurrencyData.Value = Math.Min(uiTemplateCurrencyData.MaxValue, value);
            return uiTemplateCurrencyData.Value;
        }

        public UITemplateItemData GetFirstItem(string category = null, UITemplateItemData.UnlockType unlockType = UITemplateItemData.UnlockType.All, IComparer<UITemplateItemData> orderBy = null,
            params UITemplateItemData.Status[] statuses)
        {
            return this.GetAllItems(category, unlockType, orderBy, statuses).FirstOrDefault();
        }

        public List<UITemplateItemData> GetAllItems(string category = null, UITemplateItemData.UnlockType unlockType = UITemplateItemData.UnlockType.All, IComparer<UITemplateItemData> orderBy = null,
            params UITemplateItemData.Status[] statuses)
        {
            var query                                                  = this.uiTemplateInventoryData.IDToItemData.Values.ToList();
            if (category is not null) query                            = query.Where(itemData => itemData.ItemBlueprintRecord.Category.Equals(category)).ToList();
            if (unlockType != UITemplateItemData.UnlockType.All) query = query.Where(itemData => (itemData.ShopBlueprintRecord.UnlockType & unlockType) != 0).ToList();
            if (statuses.Length > 0) query                             = query.Where(itemData => statuses.Contains(itemData.CurrentStatus)).ToList();
            if (orderBy is not null) query                             = query.OrderBy(itemData => itemData, orderBy).ToList();

            return query;
        }

        public List<UITemplateItemData> GetAllItemWithOrder(string category = null, UITemplateItemData.UnlockType unlockType = UITemplateItemData.UnlockType.All,
            IComparer<UITemplateItemData> comparer = null)
        {
            return this.GetAllItems(category, unlockType).OrderBy(itemData => itemData, comparer ?? UITemplateItemData.DefaultComparerInstance).ToList();
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
            //remove item that don't exist in blueprint anymore
            foreach (var notExistKey in this.uiTemplateInventoryData.IDToItemData.Keys.Where(itemKey => !this.uiTemplateItemBlueprint.ContainsKey(itemKey)).ToList())
            {
                this.uiTemplateInventoryData.IDToItemData.Remove(notExistKey);
            }

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

            // Set default item
            var defaultItemWithCategory = this.GetDefaultItemWithCategory();

            foreach (var (category, defaultItems) in defaultItemWithCategory)
            {
                var currentItemSelected = this.GetCurrentItemSelected(category);
                if (currentItemSelected is not null && this.uiTemplateItemBlueprint.ContainsKey(currentItemSelected)) continue;
                if (defaultItems is null or { Count: 0 }) continue;
                this.UpdateCurrentSelectedItem(category, defaultItems[0].Id);
            }
        }

        public async UniTask AddGenericReward(string rewardKey, int rewardValue, RectTransform startPosCurrency = null, string claimSoundKey = null)
        {
            if (this.uiTemplateCurrencyBlueprint.TryGetValue(rewardKey, out _))
            {
                await this.AddCurrency(rewardValue, rewardKey, startPosCurrency, claimSoundKey);
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

        public async UniTask AddGenericReward(Dictionary<string, int> reward, RectTransform startPosCurrency = null)
        {
            List<UniTask> rewardAnimationTasks = new();
            foreach (var (rewardKey, rewardValue) in reward)
            {
                rewardAnimationTasks.Add(this.AddGenericReward(rewardKey, rewardValue, startPosCurrency));
            }

            await UniTask.WhenAll(rewardAnimationTasks);
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

        public void Initialize() { this.signalBus.Subscribe<LoadBlueprintDataSucceedSignal>(this.OnLoadBlueprintSuccess); }

        public bool IsAffordCurrency(Dictionary<string, int> currency, int time = 1)
        {
            foreach (var (currencyKey, currencyValue) in currency)
            {
                if (this.GetCurrencyValue(currencyKey) < currencyValue * time)
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsAffordCurrency(string currencyName, int amount)
        {
            return this.GetCurrencyValue(currencyName) >= amount;
        }
    }
}