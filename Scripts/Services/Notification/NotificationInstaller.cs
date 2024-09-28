#if GDK_ZENJECT
namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using Zenject;

    public class NotificationInstaller : Installer<NotificationInstaller>
    {
        public override void InstallBindings()
        {
#if THEONE_NOTIFICATION && UNITY_ANDROID
            this.Container.Bind(typeof(INotificationService), typeof(IDisposable)).To<AndroidUnityNotificationService>().AsCached().NonLazy();
#elif THEONE_NOTIFICATION && UNITY_IOS
            this.Container.Bind(typeof(INotificationService), typeof(IDisposable)).To<IOSUnityNotificationService>().AsCached().NonLazy();
#else
            this.Container.Bind<INotificationService>().To<DummyNotificationService>().AsCached().NonLazy();
#endif
            this.Container.Bind<NotificationMappingHelper>().AsCached().NonLazy();
        }
    }
}
#endif