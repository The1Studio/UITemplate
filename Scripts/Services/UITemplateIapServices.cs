namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using System.Collections.Generic;
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
                this.OnPurchaseComplete(source, productId);
                onComplete?.Invoke(x);
            }, onFail);
        }

        private void OnPurchaseComplete(GameObject source, string productId)
        {
            var dataShopPackRecord = this.uiTemplateShopPackBlueprint[productId];
            var rewardItemDatas    = new Dictionary<string, UITemplateRewardItemData>();

            foreach (var rewardIdToData in dataShopPackRecord.RewardIdToRewardDatas.Values)
            {
                rewardItemDatas.Add(rewardIdToData.RewardId, new UITemplateRewardItemData(rewardIdToData.RewardValue, rewardIdToData.Repeat, rewardIdToData.AddressableFlyingItem));
            }

            if (rewardItemDatas.Count > 0)
            {
                this.signalBus.Fire(new UITemplateAddRewardsSignal(rewardItemDatas, source));
            }
        }

        public void RestorePurchase(Action onComplete = null) { this.unityIapServices.RestorePurchases(onComplete); }
    }
}