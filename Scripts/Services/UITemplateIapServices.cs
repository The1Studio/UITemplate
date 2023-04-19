namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BlueprintFlow.Signals;
    using ServiceImplementation.IAPServices;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using TheOneStudio.UITemplate.UITemplate.Services.RewardHandle;
    using UnityEngine;
    using Zenject;

    public class UITemplateIapServices
    {
        private readonly SignalBus                   signalBus;
        private readonly UITemplateShopPackBlueprint uiTemplateShopPackBlueprint;
        private readonly IUnityIapServices           unityIapServices;

        public UITemplateIapServices(SignalBus signalBus, UITemplateShopPackBlueprint uiTemplateShopPackBlueprint,
            IUnityIapServices unityIapServices)
        {
            this.signalBus                   = signalBus;
            this.uiTemplateShopPackBlueprint = uiTemplateShopPackBlueprint;
            this.unityIapServices            = unityIapServices;
            this.signalBus.Subscribe<LoadBlueprintDataSucceedSignal>(this.OnBlueprintLoaded);
            this.signalBus.Subscribe<UnityIAPOnRestorePurchaseCompleteSignal>(this.OnHandleRestorePurchase);
        }

        private void OnBlueprintLoaded(LoadBlueprintDataSucceedSignal obj)
        {
            var dicData         = new Dictionary<string, IAPModel>();
            var currentPlatForm = "Android";
#if UNITY_IOS||UNITY_IPHONE
            currentPlatForm = "IOS";
#endif

            foreach (var record in this.uiTemplateShopPackBlueprint.Values)
            {
                if (!record.Platforms.Contains(currentPlatForm))
                    continue;

                dicData.Add(record.Id, new IAPModel()
                {
                    Id          = record.Id,
                    ProductType = record.ProductType
                });
            }

            this.unityIapServices.InitIapServices(dicData);
        }

        public void BuyProduct(GameObject source, string productId, Action<string> onComplete = null, Action<string> onFail = null)
        {
            this.unityIapServices.BuyProductID(productId, (x) =>
            {
                this.OnPurchaseComplete(productId, source);
                onComplete?.Invoke(x);
            }, onFail);
        }

        private void OnPurchaseComplete(string productId, GameObject source)
        {
            var dataShopPackRecord = this.uiTemplateShopPackBlueprint[productId];
            var rewardItemDatas    = new Dictionary<string, UITemplateRewardItemData>();

            foreach (var rewardIdToData in dataShopPackRecord.RewardIdToRewardDatas.Values)
            {
                rewardItemDatas.Add(rewardIdToData.RewardId, new UITemplateRewardItemData(rewardIdToData.RewardValue, rewardIdToData.Repeat, rewardIdToData.AddressableFlyingItem));
            }

            if (rewardItemDatas.Count > 0)
            {
                this.signalBus.Fire(new UITemplateAddRewardsSignal(productId, rewardItemDatas, source));
            }
        }

        private void OnHandleRestorePurchase(UnityIAPOnRestorePurchaseCompleteSignal obj) { this.OnPurchaseComplete(obj.ProductID, null); }

        public void RestorePurchase(Action onComplete = null) { this.unityIapServices.RestorePurchases(onComplete); }

        public bool IsProductOwned(string productId = "")
        {
            foreach (var shopPackRecord in this.uiTemplateShopPackBlueprint.Values.Where(x => x.RewardIdToRewardDatas.Count > 1))
            {
                if (this.unityIapServices.IsProductOwned(shopPackRecord.Id))
                {
                    return true;
                }
            }

            return false;
        }
    }
}