#nullable enable
namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices
{
    using System;
    using Core.AdsServices;
    using Zenject;

    public enum AdType
    {
        Banner,
        Interstitial,
        Rewarded,
    }

    public sealed class AdServiceOrder
    {
        public Type   ServiceType { get; }
        public AdType AdType      { get; }
        public int    Order       { get; }

        public AdServiceOrder(Type serviceType, AdType adType, int order)
        {
            this.ServiceType = serviceType;
            this.AdType      = adType;
            this.Order       = order;
        }
    }

    public static class AdServicePriorityExtensions
    {
        public static void BindAdServiceOrder<T>(this DiContainer container, AdType adType, int order) where T : IAdServices
        {
            container.Bind<AdServiceOrder>().AsSingle().WithArguments(typeof(T), adType, order);
        }
    }
}