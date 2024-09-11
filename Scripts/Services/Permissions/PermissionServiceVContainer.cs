#if GDK_VCONTAINER
#nullable enable
namespace TheOneStudio.Permission
{
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Services.Permissions;
    using TheOneStudio.UITemplate.UITemplate.Services.Permissions.Signals;
    using VContainer;

    public static class PermissionServiceVContainer
    {
        public static void RegisterPermissionService(this IContainerBuilder builder)
        {
            #if UNITY_ANDROID
            builder.Register<AndroidPermissionService>(Lifetime.Singleton).AsImplementedInterfaces();
            #elif UNITY_IOS
            builder.Register<IOSPermissionService>(Lifetime.Singleton).AsImplementedInterfaces();
            #else
            builder.Register<DummyPermissionService>(Lifetime.Singleton).AsImplementedInterfaces();
            #endif

            builder.DeclareSignal<OnRequestPermissionStartSignal>();
            builder.DeclareSignal<OnRequestPermissionCompleteSignal>();
        }
    }
}
#endif