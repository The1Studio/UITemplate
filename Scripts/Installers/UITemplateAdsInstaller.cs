#if GDK_ZENJECT
namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using TheOneStudio.UITemplate.UITemplate.Scripts.Services;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TheOneStudio.UITemplate.UITemplate.Services.AnalyticHandler;
    using Zenject;
#if APPLOVIN
#endif
#if ADMOB || IRONSOURCE
#endif

    public class UITemplateAdsInstaller : Installer<UITemplateAdsInstaller>
    {
        private const string MinPauseSecondsToShowAoaRemoteConfigKey = "min_pause_seconds_to_show_aoa";

        public override void InstallBindings()
        {
#if !THEONE_PLAYABLE_ADS
    #if BRAVESTARS
            this.Container.BindInterfacesAndSelfTo<BraveStarsAnalyticHandler>().AsCached();
    #else
            this.Container.BindInterfacesAndSelfTo<UITemplateAnalyticHandler>().AsCached();
    #endif
#endif
#if CREATIVE
            this.Container.Bind<UITemplateAdServiceWrapper>().To<UITemplateAdServiceWrapperCreative>().AsCached();
#else
            this.Container.BindInterfacesAndSelfTo<UITemplateAdServiceWrapper>().AsCached();
#endif

#if CREATIVE
            this.Container.BindInterfacesAndSelfTo<CreativeService>().AsCached().NonLazy();
#endif
        }
    }
}
#endif