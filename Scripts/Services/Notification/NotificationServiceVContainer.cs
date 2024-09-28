#if GDK_VCONTAINER
#nullable enable
namespace TheOneStudio.Notification
{
    using TheOneStudio.UITemplate.UITemplate.Services;
    using VContainer;

    public static class NotificationServiceVContainer
    {
        public static void RegisterNotificationService(this IContainerBuilder builder)
        {
            #if THEONE_NOTIFICATION && UNITY_ANDROID
            builder.Register<AndroidUnityNotificationService>(Lifetime.Singleton).AsImplementedInterfaces();
            #elif THEONE_NOTIFICATION && UNITY_IOS
            builder.Register<IOSUnityNotificationService>(Lifetime.Singleton).AsImplementedInterfaces();
            #else
            builder.Register<DummyNotificationService>(Lifetime.Singleton).AsImplementedInterfaces();
            #endif
            builder.Register<NotificationMappingHelper>(Lifetime.Singleton);
        }
    }
}
#endif