#if GDK_ZENJECT
namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using ServiceImplementation.IAPServices;
    using TheOneStudio.UITemplate.UITemplate.Creative.Cheat;
    using TheOneStudio.UITemplate.UITemplate.Scenes.BadgeNotify;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.CollectionNew;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward.Item;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward.Pack;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TheOneStudio.UITemplate.UITemplate.Services.BreakAds;
    using TheOneStudio.UITemplate.UITemplate.Services.DeepLinking;
    using TheOneStudio.UITemplate.UITemplate.Services.Permissions;
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

            UITemplateDailyRewardInstaller.Install(this.Container);
            UITemplateDeclareSignalInstaller.Install(this.Container);
            UITemplateServicesInstaller.Install(this.Container, this.toastCanvas);
            IapInstaller.Install(this.Container);
            UITemplateLocalDataInstaller.Install(this.Container); // bind after FBInstantInstaller for remote user data
            UITemplateThirdPartyInstaller.Install(this.Container); // bind after UITemplateLocalDataInstaller for local data analytics
            UITemplateAdsInstaller.Install(this.Container); // this depend on third party service signals
            NotificationInstaller.Install(this.Container);
            StoreRatingServiceInstaller.Install(this.Container);
            PermissionServiceInstaller.Install(this.Container);
            DeepLinkInstaller.Install(this.Container);
            this.Container.BindInterfacesAndSelfTo<UITemplateIapServices>().AsCached().NonLazy();

            TheOneCheatInstaller.Install(this.Container);

            // not lock in editor because interstitial fake ads can not close
#if !UNITY_EDITOR
            this.Container.BindInterfacesAndSelfTo<LockInputService>().AsCached().NonLazy();
#endif

            //Feature
#if THEONE_DAILY_REWARD
            this.Container.BindInterfacesAndSelfTo<UITemplateDailyRewardService>().AsCached().NonLazy();
#endif

#if THEONE_NO_INTERNET
            this.Container.BindInterfacesAndSelfTo<UITemplateNoInternetService>().AsCached().NonLazy();
#endif

#if THEONE_RATE_US
            this.Container.BindInterfacesAndSelfTo<UITemplateRateUsService>().AsCached().NonLazy();
#endif

#if THEONE_BADGE_NOTIFY
            this.Container.BindInterfacesAndSelfTo<UITemplateBadgeNotifySystem>().AsCached().NonLazy();
#endif

#if THEONE_DEBUG && !PRODUCTION
            this.Container.Bind<Reporter>().FromComponentInNewPrefabResource("LogsViewer").AsCached().NonLazy();
#endif
        }
    }
}
#endif