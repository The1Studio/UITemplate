namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using Zenject;

    public class NotificationInstaller : Installer<NotificationInstaller>
    {
        public override void InstallBindings()
        {
#if NOTIFICATION && UNITY_ANDROID
            this.Container.Bind<INotificationService>().To<AndroidUnityNotificationService>().AsCached().NonLazy();
#elif NOTIFICATION && UNITY_IOS
            this.Container.Bind<INotificationService>().To<IOSUnityNotificationService>().AsCached().NonLazy();
#else
            this.Container.Bind<INotificationService>().To<DummyNotificationService>().AsCached().NonLazy();
#endif
            this.Container.Bind<NotificationMappingHelper>().AsCached().NonLazy();
        }
    }
}