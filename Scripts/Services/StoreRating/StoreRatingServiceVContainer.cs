#if GDK_VCONTAINER
#nullable enable
namespace TheOneStudio.StoreRating
{
    using TheOneStudio.UITemplate.UITemplate.Services.StoreRating;
    using VContainer;

    public static class StoreRatingServiceVContainer
    {
        public static void RegisterStoreRatingService(this IContainerBuilder builder)
        {
            #if !UNITY_EDITOR && UNITY_ANDROID && THEONE_STORE_RATING
            builder.Register<AndroidStoreRatingService>(Lifetime.Singleton).AsImplementedInterfaces();
            #elif !UNITY_EDITOR && UNITY_IOS && THEONE_STORE_RATING
            builder.Register<IosStoreRatingService>(Lifetime.Singleton).AsImplementedInterfaces();
            #else
            builder.Register<DummyStoreRatingService>(Lifetime.Singleton).AsImplementedInterfaces();
            #endif
            builder.Register<UITemplateStoreRatingHandler>(Lifetime.Singleton);
        }
    }
}
#endif