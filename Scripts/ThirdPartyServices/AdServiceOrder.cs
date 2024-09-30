#nullable enable
namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices
{
    using System;
    using Core.AdsServices;
    #if GDK_ZENJECT
    using Zenject;
    #endif
    #if GDK_VCONTAINER
    using VContainer;
    #endif

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
        #if GDK_ZENJECT
        public static void BindAdServiceOrder<T>(this DiContainer container, AdType adType, int order) where T : IAdServices
        {
            container.Bind<AdServiceOrder>().FromMethod(() => new(typeof(T), adType, order)).AsSingle();
        }
        #endif

        #if GDK_VCONTAINER
        public static void RegisterAdServiceOrder<T>(this IContainerBuilder builder, AdType adType, int order) where T : IAdServices
        {
            builder.Register<AdServiceOrder>(_ => new(typeof(T), adType, order), Lifetime.Singleton);
        }
        #endif
    }
}