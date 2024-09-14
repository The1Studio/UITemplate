#if GDK_VCONTAINER
#nullable enable
namespace TheOneStudio.UITemplate
{
    using GameFoundation.DI;
    using TheOneStudio.DeepLinking;
    using TheOneStudio.Notification;
    using TheOneStudio.Permission;
    using TheOneStudio.StoreRating;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.CollectionNew;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TheOneStudio.UITemplate.UITemplate.Services.BreakAds;
    using TheOneStudio.UITemplate.UITemplate.Services.Toast;
    using UnityEngine;
    using VContainer;

    public static class UITemplateVContainer
    {
        public static void RegisterUITemplate(this IContainerBuilder builder, Transform rootTransform, ToastController toastControllerPrefab)
        {
            Application.targetFrameRate = 60;

            builder.Register<UITemplateAnimationHelper>(Lifetime.Singleton);
            builder.Register<UITemplateCollectionItemViewHelper>(Lifetime.Singleton);
            builder.Register<BreakAdsViewHelper>(Lifetime.Singleton);

            builder.RegisterUITemplateAdsService();
            builder.RegisterUITemplateThirdPartyServices();

            builder.RegisterUITemplateLocalData();
            builder.RegisterUITemplateServices(rootTransform, toastControllerPrefab);
            builder.RegisterUITemplateDailyReward();
            builder.RegisterNotificationService();
            builder.RegisterStoreRatingService();
            builder.RegisterPermissionService();
            builder.RegisterDeepLinkService();

            builder.DeclareUITemplateSignals();

            builder.Register<UITemplateIapServices>(Lifetime.Singleton).AsInterfacesAndSelf();

            // TheOneCheatInstaller.Install(this.Container);

            // not lock in editor because interstitial fake ads can not close
            #if !UNITY_EDITOR
            builder.Register<LockInputService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            #endif

            //Feature
            #if THEONE_DAILY_REWARD
            builder.Register<UITemplateDailyRewardService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            #endif

            #if THEONE_NO_INTERNET
            builder.Register<UITemplateNoInternetService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            #endif

            #if THEONE_RATE_US
            builder.Register<UITemplateRateUsService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            #endif

            #if THEONE_BADGE_NOTIFY
            builder.Register<UITemplateBadgeNotifySystem>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            #endif

            #if THEONE_DEBUG && !PRODUCTION
            builder.RegisterComponentInNewPrefabResource<Reporter>("LogsViewer", Lifetime.Singleton).UnderTransform(rootTransform);
            #endif
        }
    }
}
#endif