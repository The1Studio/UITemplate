namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using TheOneStudio.UITemplate.UITemplate.Interfaces;

    public class UITemplateIAPSystem : IIapSystem
    {
        public void BuyProduct(string productId, Action onComplete = null) { onComplete?.Invoke(); }

        public void BuyRemoveAds(Action onComplete = null) { onComplete?.Invoke(); }
    }
}