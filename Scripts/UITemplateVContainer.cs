#if GDK_VCONTAINER
#nullable enable
namespace TheOneStudio.UITemplate
{
    using System.Linq;
    using GameFoundation.DI;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.DeepLinking;
    using TheOneStudio.Notification;
    using TheOneStudio.Permission;
    using TheOneStudio.StoreRating;
    using TheOneStudio.UITemplate.UITemplate.Creative.Cheat;
    using TheOneStudio.UITemplate.UITemplate.Scenes.BadgeNotify;
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
            builder.Register(typeof(BreakAdsViewHelper).GetDerivedTypes().OrderBy(type => type == typeof(BreakAdsViewHelper)).First(), Lifetime.Singleton).As<BreakAdsViewHelper>();

            builder.RegisterUITemplateAdsService();
            builder.RegisterUITemplateThirdPartyServices();

            builder.RegisterUITemplateLocalData();
            builder.RegisterUITemplateServices(rootTransform, toastControllerPrefab);
            builder.RegisterDailyReward();
            builder.RegisterNotificationService();
            builder.RegisterStoreRatingService();
            builder.RegisterPermissionService();
            builder.RegisterDeepLinkService();

            builder.DeclareUITemplateSignals();

            builder.Register<UITemplateIapServices>(Lifetime.Singleton).AsInterfacesAndSelf();

            builder.RegisterCheatView();

            // not lock in editor because interstitial fake ads can not close
            #if !UNITY_EDITOR && !DISABLE_LOCK_INPUT
            builder.Register<LockInputService>(Lifetime.Singleton).AsInterfacesAndSelf();
            #endif

            //Feature
            #if THEONE_DAILY_REWARD
            builder.Register<UITemplateDailyRewardService>(Lifetime.Singleton).AsInterfacesAndSelf();
            #endif

            #if THEONE_NO_INTERNET
            builder.Register<UITemplateNoInternetService>(Lifetime.Singleton).AsInterfacesAndSelf();
            #endif

            #if THEONE_RATE_US
            builder.Register<UITemplateRateUsService>(Lifetime.Singleton).AsInterfacesAndSelf();
            #endif

            #if THEONE_BADGE_NOTIFY
            builder.Register<UITemplateBadgeNotifySystem>(Lifetime.Singleton).AsInterfacesAndSelf();
            #endif

            #if THEONE_DEBUG && !PRODUCTION
            builder.RegisterComponentInNewPrefabResource<Reporter>("LogsViewer", Lifetime.Singleton).UnderTransform(rootTransform);
            builder.AutoResolve<Reporter>();
            #endif
        }
    }
}
#endif