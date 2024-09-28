#if GDK_VCONTAINER
#nullable enable
namespace TheOneStudio.UITemplate
{
    using GameFoundation.DI;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.Helpers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.FeaturesConfig;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Services;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TheOneStudio.UITemplate.UITemplate.Services.RewardHandle;
    using TheOneStudio.UITemplate.UITemplate.Services.RewardHandle.AllRewards;
    using TheOneStudio.UITemplate.UITemplate.Services.Toast;
    using TheOneStudio.UITemplate.UITemplate.Services.Vibration;
    using TheOneStudio.UITemplate.UITemplate.Utils;
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;

    public static class UITemplateServicesVContainer
    {
        public static void RegisterUITemplateServices(this IContainerBuilder builder, Transform rootTransform, ToastController toastController)
        {
            builder.Register<UITemplateFeatureConfig>(Lifetime.Singleton).AsInterfacesAndSelf();

            // Master Audio
            builder.Register<UITemplateSoundServices>(Lifetime.Singleton);

            //Build-in service
            builder.Register<InternetService>(Lifetime.Singleton).AsImplementedInterfaces();

            //HandleScreenShow
            builder.Register<UITemplateScreenShowServices>(Lifetime.Singleton).AsImplementedInterfaces();
            typeof(IUITemplateScreenShow).GetDerivedTypes().ForEach(type => builder.Register(type, Lifetime.Singleton).AsImplementedInterfaces());

            //FlyingAnimation Currency
            builder.Register<UITemplateFlyingAnimationController>(Lifetime.Singleton);

            //Utils
            builder.Register<GameAssetUtil>(Lifetime.Singleton);

            //Vibration
            builder.Register<UITemplateVibrationService>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.Register<UITemplateHandleSoundWhenOpenAdsServices>(Lifetime.Singleton);

            //Reward Handle
            builder.Register<UITemplateRewardHandler>(Lifetime.Singleton);
            typeof(IUITemplateRewardExecutor).GetDerivedTypes().ForEach(type => builder.Register(type, Lifetime.Singleton).AsImplementedInterfaces());

            // VFX Spawn
            builder.Register<UITemplateVFXSpawnService>(Lifetime.Singleton);

            // Toast
            builder.RegisterComponentInNewPrefab(toastController, Lifetime.Singleton).UnderTransform(rootTransform);

            //Button experience helper
            builder.Register<UITemplateButtonExperienceHelper>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}
#endif