namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BlueprintFlow.Signals;
    using GameFoundation.DI;
    using GameFoundation.Signals;
    using ServiceImplementation.IAPServices;
    using ServiceImplementation.IAPServices.Signals;
    using TheOne.Logging;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using TheOneStudio.UITemplate.UITemplate.Services.RewardHandle;
    using UnityEngine;
    using UnityEngine.Scripting;
    using ILogger = TheOne.Logging.ILogger;

    public class UITemplateIapServices : IInitializable, IDisposable
    {
        #region inject

        private readonly SignalBus                            signalBus;
        private readonly ILogger                              logger;
        private readonly UITemplateIAPOwnerPackControllerData uiTemplateIAPOwnerPackControllerData;
        private readonly UITemplateShopPackBlueprint          uiTemplateShopPackBlueprint;
        private readonly IIapServices                         iapServices;
        private readonly UITemplateRewardHandler              uiTemplateRewardHandler;

        #endregion

        [Preserve]
        public UITemplateIapServices(
            SignalBus                            signalBus,
            ILoggerManager                       loggerManager,
            UITemplateIAPOwnerPackControllerData uiTemplateIAPOwnerPackControllerData,
            UITemplateShopPackBlueprint          uiTemplateShopPackBlueprint,
            IIapServices                         iapServices,
            UITemplateRewardHandler              uiTemplateRewardHandler
        )
        {
            this.signalBus                            = signalBus;
            this.logger                               = loggerManager.GetLogger(this);
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
                dicData.Add(record.Id,
                    new()
                    {
                        Id          = record.Id,
                        ProductType = record.ProductType,
                    });
            }

            this.iapServices.InitIapServices(dicData);
        }

        public void BuyProduct(GameObject source, string productId, Action<string, int> onComplete = null, Action<string> onFail = null)
        {
            this.logger.Warning($"BuyProduct {productId}");

            this.iapServices.BuyProductID(productId,
                (x, quantity) =>
                {
                    this.OnPurchaseComplete(productId, source, quantity);
                    onComplete?.Invoke(x, quantity);
                },
                onFail);
        }

        private void OnPurchaseComplete(string productId, GameObject source, int quantity)
        {
            var dataShopPackRecord = this.uiTemplateShopPackBlueprint[productId];
            this.uiTemplateIAPOwnerPackControllerData.AddPack(productId);

            var rewardItemData = dataShopPackRecord.RewardIdToRewardDatas.ToDictionary(keyPairValue => keyPairValue.Key,
                keyPairValue => new UITemplateRewardItemData(keyPairValue.Value.RewardValue, keyPairValue.Value.Repeat, keyPairValue.Value.AddressableFlyingItem));

            if (rewardItemData.Count <= 0) return;
            for (var i = 0; i < quantity; i++)
            {
                this.uiTemplateRewardHandler.AddRewardsWithPackId(productId, rewardItemData, $"iap_{productId}", source);
            }
        }

        private void OnHandleRestorePurchase(OnRestorePurchaseCompleteSignal obj)
        {
            this.OnPurchaseComplete(obj.ProductID, null, obj.Quantity);
        }

        public void RestorePurchase(Action onComplete = null, Action onFail = null)
        {
            Action onCompleteCallback = () => this.uiTemplateIAPOwnerPackControllerData.SetRestoredPurchase();
            onCompleteCallback += onComplete;
            this.iapServices.RestorePurchases(onCompleteCallback, onFail);
        }

        public bool IsProductOwned(string productId)
        {
            return this.uiTemplateShopPackBlueprint.Values.FirstOrDefault(x => x.Id.Equals(productId)) != null && this.uiTemplateIAPOwnerPackControllerData.IsOwnerPack(productId);
        }

        public ProductData GetProductData(string productId)
        {
            return this.iapServices.GetProductData(productId);
        }

        public string GetPriceById(string productId, string defaultPrice)
        {
            return this.iapServices.GetPriceById(productId, defaultPrice);
        }

        public bool IsRestoredPurchase() => this.uiTemplateIAPOwnerPackControllerData.IsRestoredPurchase();

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