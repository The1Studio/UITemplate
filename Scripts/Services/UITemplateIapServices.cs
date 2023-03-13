#if TEMPLATE_IAP
namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using System.Linq;
    using BlueprintFlow.Signals;
    using Core.AdsServices;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Interfaces;
    using Unity.Services.Core;
    using Unity.Services.Core.Environments;
    using UnityEngine;
    using UnityEngine.Purchasing;
    using Zenject;
    using ProductType = UnityEngine.Purchasing.ProductType;

    public class UITemplateIapServices : IIapServices, IStoreListener
    {
        private        Action<string>     onPurchaseComplete, onPurchaseFailed;
        private static IStoreController   mStoreController;
        private static IExtensionProvider mStoreExtensionProvider;

        private readonly SignalBus         signalBus;
        private readonly ILogService       logger;
        private readonly ShopPackBlueprint iapPackBlueprint;
        private readonly IAdServices       adServices;
        private const    string            Environment = "production";

        public UITemplateIapServices(SignalBus signalBus, ILogService log, ShopPackBlueprint shopPack, IAdServices adServices)
        {
            this.signalBus = signalBus;
            this.logger = log;
            this.iapPackBlueprint = shopPack;
            this.adServices = adServices;

            this.signalBus.Subscribe<LoadBlueprintDataSucceedSignal>(this.OnLoadBlueprintDataSuccess);
        }

        private async void OnLoadBlueprintDataSuccess(LoadBlueprintDataSucceedSignal obj)
        {
            if (mStoreController == null)
            {
                // Begin to configure our connection to Purchasing
                try
                {
                    var options = new InitializationOptions()
                        .SetEnvironmentName(Environment);

                    await UnityServices.InitializeAsync(options);
                }
                catch (Exception exception)
                {
                    // An error occurred during services initialization.
                    Debug.Log($"init failed {exception.Message}");
                }

                this.InitializePurchasing();
            }
        }

        private bool IsInitialized => mStoreController != null && mStoreExtensionProvider != null;

        private void InitializePurchasing()
        {
            if (this.IsInitialized)
            {
                return;
            }

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            this.AddAllProduct(builder);
            UnityPurchasing.Initialize(this, builder);
        }

        private void AddAllProduct(ConfigurationBuilder builder)
        {
            for (var i = 0; i < this.iapPackBlueprint.Count; i++)
            {
                var current = this.iapPackBlueprint.ElementAt(i);
                builder.AddProduct(current.Value.Id, this.ConvertToUnityProductType(current.Value.ProductType));
            }
        }

        private ProductType ConvertToUnityProductType(Blueprints.ProductType productType)
        {
            var type = productType switch
            {
                Blueprints.ProductType.Consumable => ProductType.Consumable,
                Blueprints.ProductType.Subscription => ProductType.Subscription,
                Blueprints.ProductType.NonConsumable => ProductType.NonConsumable,
                Blueprints.ProductType.RemoveAds => ProductType.NonConsumable,
                _ => ProductType.Consumable
            };

            return type;
        }

        public string GetPriceById(string id)
        {
            var s = "";

            if (!this.IsInitialized) return s;

            try
            {
                s = mStoreController.products.WithID(id).metadata.localizedPriceString;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return s;
        }

        public void BuyRemoveAds(Action completed = null, Action<string> onFailed = null)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                //todo show no internet connection
            }
            else
            {
                if (mStoreController == null)
                {
                    // Begin to configure our connection to Purchasing
                    this.InitializePurchasing();
                }
                else
                {
                    this.BuyProductID(this.iapPackBlueprint.ElementAt(0).Key, (x) => { completed?.Invoke(); },
                        onFailed);
                }
            }
        }

        public void BuyProductID(string productId, Action<string> onComplete = null, Action<string> onFailed = null)
        {
            if (this.IsInitialized)
            {
                var product = mStoreController.products.WithID(productId);

                if (product is { availableToPurchase: true })
                {
                    this.logger.Log($"Purchasing product asychronously: '{product.definition.id}'");

                    this.onPurchaseComplete = onComplete;
                    mStoreController.InitiatePurchase(product);
                }
                else
                {
                    this.onPurchaseFailed = onFailed;
                    this.logger.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            else
            {
                this.InitializePurchasing();
                this.logger.Log("BuyProductID FAIL. Not initialized.");
            }
        }

        // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
        // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
        public void RestorePurchases()
        {
            // If Purchasing has not yet been set up ...
            if (!this.IsInitialized)
            {
                // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
                this.logger.Log("RestorePurchases FAIL. Not initialized.");

                return;
            }

            // If we are running on an Apple device ... 
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                // ... begin restoring purchases
                this.logger.Log("RestorePurchases started ...");

                // Fetch the Apple store-specific subsystem.
                var apple = mStoreExtensionProvider.GetExtension<IAppleExtensions>();

                // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
                // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
                apple.RestoreTransactions((result, message) =>
                {
                    // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                    // no purchases are available to be restored.
                    this.logger.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                });
            }
            // Otherwise ...
            else
            {
                // We are not running on an Apple device. No work is necessary to restore purchases.
                this.logger.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            }
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            this.logger.Log("OnInitialized: PASS");
            mStoreController = controller;
            mStoreExtensionProvider = extensions;
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message) { }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
            this.logger.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            foreach (var iapPackRecord in this.iapPackBlueprint.Values.Where(iapPackRecord => string.Equals(args.purchasedProduct.definition.id, iapPackRecord.Id, StringComparison.Ordinal)))
            {
                if (iapPackRecord.Id.Equals(this.iapPackBlueprint.ElementAt(0).Key))
                {
                    this.adServices.RemoveAds();

                    break;
                }
            }

            this.onPurchaseComplete?.Invoke(args.purchasedProduct.definition.id);
            this.onPurchaseComplete = null;

            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            this.onPurchaseFailed?.Invoke(product.definition.storeSpecificId);
            this.onPurchaseFailed = null;
            this.logger.Log($"OnPurchaseFailed: FAIL. Product: '{product.definition.storeSpecificId}', PurchaseFailureReason: {failureReason}");
        }
    }

}
#endif