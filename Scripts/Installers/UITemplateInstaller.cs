namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using ServiceImplementation.IAPServices;
    using TheOneStudio.UITemplate.HighScore;
    using TheOneStudio.UITemplate.UITemplate.Scenes.BadgeNotify;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.CollectionNew;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TheOneStudio.UITemplate.UITemplate.Services.AppTracking;
    using TheOneStudio.UITemplate.UITemplate.Services.BreakAds;
    using TheOneStudio.UITemplate.UITemplate.Services.StoreRating;
    using TheOneStudio.UITemplate.UITemplate.Services.Toast;
    using UnityEngine;
    using Zenject;

    public class UITemplateInstaller : Installer<ToastController, UITemplateInstaller>
    {
        private readonly ToastController toastCanvas;

        public UITemplateInstaller(ToastController toastCanvas) { this.toastCanvas = toastCanvas; }

        public override void InstallBindings()
        {
            Application.targetFrameRate = 60;
            //Helper
            this.Container.Bind<UITemplateAnimationHelper>().AsCached();
            this.Container.Bind<UITemplateCollectionItemViewHelper>().AsCached();
            this.Container.Bind<BreakAdsViewHelper>().AsCached();

            UITemplateDeclareSignalInstaller.Install(this.Container);
            UITemplateServicesInstaller.Install(this.Container, this.toastCanvas);
            UITemplateHighScoreInstaller.Install(this.Container);
            IapInstaller.Install(this.Container);
            UITemplateLocalDataInstaller.Install(this.Container); // bind after FBInstantInstaller for remote user data
            UITemplateThirdPartyInstaller.Install(this.Container); // bind after UITemplateLocalDataInstaller for local data analytics
            UITemplateAdsInstaller.Install(this.Container); // this depend on third party service signals
            NotificationInstaller.Install(this.Container);
            StoreRatingServiceInstaller.Install(this.Container);
            this.Container.BindInterfacesAndSelfTo<UITemplateIapServices>().AsCached().NonLazy();
            this.Container.BindInterfacesAndSelfTo<AppTrackingServices>().AsCached().NonLazy();
            this.Container.BindInterfacesAndSelfTo<LockInputService>().AsCached().NonLazy();

            //Feature
#if THEONE_DAILY_REWARD
            this.Container.BindInterfacesAndSelfTo<UITemplateDailyRewardService>().AsCached().NonLazy();
#endif

#if THEONE_BADGE_NOTIFY
            this.Container.BindInterfacesAndSelfTo<UITemplateBadgeNotifySystem>().AsCached().NonLazy();
#endif
        }
    }
}