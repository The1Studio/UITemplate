namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using Zenject;

    public class NotificationInstaller : Installer<NotificationInstaller>
    {
        public override void InstallBindings()
        {
#if NOTIFICATION && UNITY_ANDROID
            this.Container.BindInterfacesAndSelfTo<AndroidUnityNotificationService>().AsCached().NonLazy();
#elif NOTIFICATION && UNITY_IOS
            this.Container.BindInterfacesAndSelfTo<IOSUnityNotificationService>().AsCached().NonLazy();
#else
            this.Container.BindInterfacesAndSelfTo<DummyNotificationService>().AsCached().NonLazy();
#endif
            this.Container.Bind<NotificationMappingHelper>().AsCached().NonLazy();
        }
    }
}