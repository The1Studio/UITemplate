namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using System.Collections.Generic;
    using Core.AdsServices;
    using GameFoundation.Scripts.Utilities.Extension;
    using global::Models;
    using ServiceImplementation.AdsServices.EasyMobile;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Services;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents;
    using Zenject;

    public class UITemplateAdsInstaller : Installer<UITemplateAdsInstaller>
    {
        public override void InstallBindings()
        {
            //AdsConfig
            this.Container.Bind<UITemplateAdServiceConfig>().AsCached().NonLazy();
            this.Container.BindInterfacesAndSelfTo<UITemplateAnalyticHandler>().AsCached();
            this.Container.BindInterfacesAndSelfToAllTypeDriveFrom<BaseAnalyticEventFactory>();
#if CREATIVE
            this.Container.Bind<UITemplateAdServiceWrapper>().To<UITemplateAdServiceWrapperCreative>().AsCached();
#else
            this.Container.BindInterfacesAndSelfTo<UITemplateAdServiceWrapper>().AsCached();
#endif

#if EM_ADMOB
            var listAoaAppId = this.Container.Resolve<GDKConfig>().GetGameConfig<AdmobAOAConfig>().ListAoaAppId;
            var listMRecId   = this.Container.Resolve<GDKConfig>().GetGameConfig<AdmobAOAConfig>().ListMRecId;

            var adMobWrapperConfig = new AdModWrapper.Config(listAoaAppId)
            {
                ADModMRecIds = new Dictionary<AdViewPosition, string>()
            };

            var listMRecAndroidAdViewPosition = this.Container.Resolve<GDKConfig>().GetGameConfig<AdmobAOAConfig>().listMRecAdViewPosition;

            for (var i = listMRecId.Count - 1; i >= 0; i--)
            {
                adMobWrapperConfig.ADModMRecIds.Add(listMRecAndroidAdViewPosition[i], listMRecId[i]);
            }

            this.Container.Bind<AdModWrapper.Config>().FromInstance(adMobWrapperConfig).WhenInjectedInto<AdModWrapper>();
#endif

#if CREATIVE && EM_ADMOB
            adMobWrapperConfig.IsShowAOAAtOpenApp = false;
            adMobWrapperConfig.OpenAOAAfterResuming = false;
#endif

#if CREATIVE
            this.Container.BindInterfacesAndSelfTo<CreativeService>().AsCached().NonLazy();
#endif
        }
    }
}