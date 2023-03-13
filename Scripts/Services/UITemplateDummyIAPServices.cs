namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using TheOneStudio.UITemplate.UITemplate.Interfaces;

    public class UITemplateDummyIAPServices : IIapServices
    {
        public void BuyProductID(string productId, Action<string> onComplete = null, Action<string> onFailed = null) { onComplete?.Invoke(productId); }

        public void   BuyRemoveAds(Action onComplete = null, Action<string> onFailed = null) { onComplete?.Invoke(); }
        public string GetPriceById(string productId)                                         { return $"0.0"; }
    }
}