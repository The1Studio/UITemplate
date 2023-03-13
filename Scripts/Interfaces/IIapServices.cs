namespace TheOneStudio.UITemplate.UITemplate.Interfaces
{
    using System;

    public interface IIapServices
    {
        void   BuyProductID(string productId, Action<string> onComplete = null, Action<string> onFailed = null);
        void   BuyRemoveAds(Action onComplete = null, Action<string> onFailed = null);
        string GetPriceById(string productId);
    }
}