namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BlueprintFlow.Signals;
    using Core.AdsServices;
    using GameFoundation.Scripts.Utilities.LogService;
    using ServiceImplementation.IAPServices;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using TheOneStudio.UITemplate.UITemplate.Services.RewardHandle;
    using UnityEngine;
    using Zenject;

    public class UITemplateIapServices : IInitializable, IDisposable
    {
        private readonly SignalBus                            signalBus;
        private readonly ILogService                          logger;
        private readonly IAdServices                          adServices;
        private readonly UITemplateIAPOwnerPackControllerData uiTemplateIAPOwnerPackControllerData;
        private readonly UITemplateShopPackBlueprint          uiTemplateShopPackBlueprint;
        private readonly IIapServices                         iapServices;

        public UITemplateIapServices(SignalBus signalBus, ILogService logger, IAdServices adServices, UITemplateIAPOwnerPackControllerData uiTemplateIAPOwnerPackControllerData,
            UITemplateShopPackBlueprint uiTemplateShopPackBlueprint,
            IIapServices iapServices)
        {
            this.signalBus                            = signalBus;
            this.logger                               = logger;
            this.adServices                           = adServices;
            this.uiTemplateIAPOwnerPackControllerData = uiTemplateIAPOwnerPackControllerData;
            this.uiTemplateShopPackBlueprint          = uiTemplateShopPackBlueprint;
            this.iapServices                          = iapServices;
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
            var rewardItemDatas    = new Dictionary<string, UITemplateRewardItemData>();
            this.uiTemplateIAPOwnerPackControllerData.AddPack(productId);

            foreach (var rewardIdToData in dataShopPackRecord.RewardIdToRewardDatas.Values)
            {
                rewardItemDatas.Add(rewardIdToData.RewardId, new UITemplateRewardItemData(rewardIdToData.RewardValue, rewardIdToData.Repeat, rewardIdToData.AddressableFlyingItem));
            }

            if (rewardItemDatas.Count > 0)
            {
                this.signalBus.Fire(new UITemplateAddRewardsSignal(productId, rewardItemDatas, source));
            }
        }

        private void OnHandleRestorePurchase(OnRestorePurchaseCompleteSignal obj) { this.OnPurchaseComplete(obj.ProductID, null); }

        public void RestorePurchase(Action onComplete = null) { this.iapServices.RestorePurchases(onComplete); }

        public bool IsProductOwned(string productId = "")
        {
            //Todo check with pack ID
            return this.uiTemplateShopPackBlueprint.Values.Where(x => x.RewardIdToRewardDatas.Count > 1).Any(shopPackRecord => this.uiTemplateIAPOwnerPackControllerData.IsOwnerPack(shopPackRecord.Id));
        }
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