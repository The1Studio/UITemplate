namespace TheOneStudio.UITemplate.UITemplate.Interfaces
{
    using System;

    public interface IIapSystem
    {
        void BuyProduct(string productId, Action onComplete = null);
        void BuyRemoveAds(Action onComplete = null);
    }
}