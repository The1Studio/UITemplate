namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BlueprintFlow.Signals;
    using GameFoundation.DI;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Signals;
    using ServiceImplementation.IAPServices;
    using ServiceImplementation.IAPServices.Signals;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using TheOneStudio.UITemplate.UITemplate.Services.RewardHandle;
    using UnityEngine;
    using UnityEngine.Scripting;

    public class UITemplateIapServices : IInitializable, IDisposable
    {
        #region inject

        private readonly SignalBus                            signalBus;
        private readonly ILogService                          logger;
        private readonly UITemplateIAPOwnerPackControllerData uiTemplateIAPOwnerPackControllerData;
        private readonly UITemplateShopPackBlueprint          uiTemplateShopPackBlueprint;
        private readonly IIapServices                         iapServices;
        private readonly UITemplateRewardHandler              uiTemplateRewardHandler;

        #endregion

        [Preserve]
        public UITemplateIapServices(
            SignalBus                            signalBus,
            ILogService                          logger,
            UITemplateIAPOwnerPackControllerData uiTemplateIAPOwnerPackControllerData,
            UITemplateShopPackBlueprint          uiTemplateShopPackBlueprint,
            IIapServices                         iapServices,
            UITemplateRewardHandler              uiTemplateRewardHandler
        )
        {
            this.signalBus                            = signalBus;
            this.logger                               = logger;
            this.uiTemplateIAPOwnerPackControllerData = uiTemplateIAPOwnerPackControllerData;
            this.uiTemplateShopPackBlueprint          = uiTemplateShopPackBlueprint;
            this.iapServices                          = iapServices;
            this.uiTemplateRewardHandler              = uiTemplateRewardHandler;
        }

        private void OnBlueprintLoaded(LoadBlueprintDataSucceedSignal obj)
        {
            var dicData = new Dictionary<string, IAPModel>();

            foreach (var record in this.uiTemplateShopPackBlueprint.GetPack())
            {
                dicData.Add(record.Id, new IAPModel()
                {
                    Id          = record.Id,
                    ProductType = record.ProductType
                });
            }

            this.iapServices.InitIapServices(dicData);
        }

        public void BuyProduct(GameObject source, string productId, Action<string> onComplete = null, Action<string> onFail = null)
        {
            this.logger.Warning($"BuyProduct {productId}");

            this.iapServices.BuyProductID(productId, (x) =>
            {
                this.OnPurchaseComplete(productId, source);
                onComplete?.Invoke(x);
            }, onFail);
        }

        private void OnPurchaseComplete(string productId, GameObject source)
        {
            var dataShopPackRecord = this.uiTemplateShopPackBlueprint[productId];
            this.uiTemplateIAPOwnerPackControllerData.AddPack(productId);

            var rewardItemData = dataShopPackRecord.RewardIdToRewardDatas.ToDictionary(keyPairValue => keyPairValue.Key,
                keyPairValue => new UITemplateRewardItemData(keyPairValue.Value.RewardValue, keyPairValue.Value.Repeat, keyPairValue.Value.AddressableFlyingItem));

            if (rewardItemData.Count > 0)
            {
                this.uiTemplateRewardHandler.AddRewardsWithPackId(productId, rewardItemData, source);
            }
        }

        private void OnHandleRestorePurchase(OnRestorePurchaseCompleteSignal obj) { this.OnPurchaseComplete(obj.ProductID, null); }

        public void RestorePurchase(Action onComplete = null) { this.iapServices.RestorePurchases(onComplete); }

        public bool IsProductOwned(string productId = "")
        {
            //Todo check with pack ID
            return this.uiTemplateShopPackBlueprint.Values.Where(x => x.RewardIdToRewardDatas.Count > 1).Any(shopPackRecord => this.uiTemplateIAPOwnerPackControllerData.IsOwnerPack(shopPackRecord.Id));
        }

        public ProductData GetProductData(string productId) { return this.iapServices.GetProductData(productId); }

        public string GetPriceById(string productId, string defaultPrice) { return this.iapServices.GetPriceById(productId, defaultPrice); }

        public void Initialize()
        {
            this.signalBus.Subscribe<LoadBlueprintDataSucceedSignal>(this.OnBlueprintLoaded);
            this.signalBus.Subscribe<OnRestorePurchaseCompleteSignal>(this.OnHandleRestorePurchase);
        }

        public void Dispose()
        {
            this.signalBus.Unsubscribe<LoadBlueprintDataSucceedSignal>(this.OnBlueprintLoaded);
            this.signalBus.Unsubscribe<OnRestorePurchaseCompleteSignal>(this.OnHandleRestorePurchase);
        }
    }
}