namespace TheOneStudio.UITemplate.UITemplate.Services.StoreRating
{
    using Zenject;

    public class StoreRatingServiceInstaller : Installer<StoreRatingServiceInstaller>
    {
        public override void InstallBindings()
        {
#if !UNITY_ANDROID && !UNITY_IOS
            this.Container.Bind<IStoreRatingService>().To<DummyStoreRatingService>().AsSingle().NonLazy();
#endif
#if UNITY_ANDROID
            this.Container.Bind<IStoreRatingService>().To<AndroidStoreRatingService>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
#endif
#if UNITY_IOS
            this.Container.Bind<IStoreRatingService>().To<IosStoreRatingService>().AsSingle().NonLazy();
#endif
            this.Container.Bind<UITemplateStoreRatingHandler>().AsSingle().NonLazy();
        }
    }
}