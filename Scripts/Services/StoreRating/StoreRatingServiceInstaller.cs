namespace TheOneStudio.UITemplate.UITemplate.Services.StoreRating
{
    using Zenject;

    public class StoreRatingServiceInstaller : Installer<StoreRatingServiceInstaller>
    {
        public override void InstallBindings()
        {
#if !UNITY_EDITOR && UNITY_ANDROID && STORE_RATING
            this.Container.Bind<IStoreRatingService>().To<AndroidStoreRatingService>().AsSingle().NonLazy();
#elif !UNITY_EDITOR && UNITY_IOS && STORE_RATING
            this.Container.Bind<IStoreRatingService>().To<IosStoreRatingService>().AsSingle().NonLazy();
#else
            this.Container.Bind<IStoreRatingService>().To<DummyStoreRatingService>().AsSingle().NonLazy();
#endif
            this.Container.Bind<UITemplateStoreRatingHandler>().AsSingle().NonLazy();
        }
    }
}