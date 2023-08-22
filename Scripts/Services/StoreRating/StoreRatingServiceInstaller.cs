namespace TheOneStudio.UITemplate.UITemplate.Services.StoreRating
{
    using Zenject;

    public class StoreRatingServiceInstaller : Installer<StoreRatingServiceInstaller>
    {
        public override void InstallBindings()
        {
#if UNITY_EDITOR
            this.Container.Bind<IStoreRatingService>().To<DummyStoreRatingService>().AsSingle().NonLazy();
#elif UNITY_ANDROID && STORE_RATING
            this.Container.Bind<IStoreRatingService>().To<AndroidStoreRatingService>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
#elif UNITY_IOS && STORE_RATING
            this.Container.Bind<IStoreRatingService>().To<IosStoreRatingService>().AsSingle().NonLazy();
#endif
            this.Container.Bind<UITemplateStoreRatingHandler>().AsSingle().NonLazy();
        }
    }
}