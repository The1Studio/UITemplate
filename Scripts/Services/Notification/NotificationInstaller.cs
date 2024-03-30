namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using Zenject;

    public class NotificationInstaller : Installer<NotificationInstaller>
    {
        public override void InstallBindings()
        {
#if THEONE_NOTIFICATION && UNITY_ANDROID
            this.Container.BindInterfacesAndSelfTo<AndroidUnityNotificationService>().AsCached().NonLazy();
#elif THEONE_NOTIFICATION && UNITY_IOS
            this.Container.BindInterfacesAndSelfTo<IOSUnityNotificationService>().AsCached().NonLazy();
#else
            this.Container.BindInterfacesAndSelfTo<DummyNotificationService>().AsCached().NonLazy();
#endif
            this.Container.Bind<NotificationMappingHelper>().AsCached().NonLazy();
        }
    }
}