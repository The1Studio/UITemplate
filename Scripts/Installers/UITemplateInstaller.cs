namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using System.Linq;
    using GameFoundation.Scripts.Utilities.LogService;
#if FB_INSTANT
    using ServiceImplementation.FBInstant;
#endif
    using ServiceImplementation.FireBaseRemoteConfig;
    using ServiceImplementation.IAPServices;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.CollectionNew;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TheOneStudio.UITemplate.UITemplate.Services.Toast;
    using UnityEngine;
    using Zenject;
#if UITEMPLATE_DECORATION
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.Decoration
#endif

    public class UITemplateInstaller : Installer<ToastController, UITemplateInstaller>
    {
        private readonly ToastController toastCanvas;

        public UITemplateInstaller(ToastController toastCanvas)
        {
            this.toastCanvas      = toastCanvas;
        }

        public override void InstallBindings()
        {
            Application.targetFrameRate = 60;
            //Helper
            this.Container.Bind<UITemplateAnimationHelper>().AsCached();
            this.Container.Bind<UITemplateCollectionItemViewHelper>().AsCached();

            UITemplateDeclareSignalInstaller.Install(this.Container);
            UnityIapInstaller.Install(this.Container);
#if FB_INSTANT
            FBInstantInstaller.Install(this.Container); // depend on UITemplateThirdPartyInstaller for signals
#endif
            UITemplateLocalDataInstaller.Install(this.Container); // bind after FBInstantInstaller for remote user data
            UITemplateThirdPartyInstaller.Install(this.Container); // bind after UITemplateLocalDataInstaller for local data analytics
            UITemplateAdsInstaller.Install(this.Container); // this depend on third party service signals

            this.EnableReporterWithDeVicesID();
            //Features: decoration, building, starter pack .....
#if UITEMPLATE_DECORATION
            this.Container.BindInterfacesAndSelfTo<UITemplateDecorationManager>().AsCached().NonLazy();
#endif
        }

        private void EnableReporterWithDeVicesID()
        {
#if !ENABLE_REPORTER

            var devicesId = SystemInfo.deviceUniqueIdentifier;
            this.Container.Resolve<ILogService>().Log($"Devices ID: {devicesId}");
            var debugKey = "DebugDevices";

            this.Container.Resolve<SignalBus>().Subscribe<RemoteConfigInitializeSucceededSignal>(() =>
            {
                var remoteConfig = this.Container.Resolve<IRemoteConfig>();
                var devices      = remoteConfig.GetRemoteConfigStringValue(debugKey, "");

                if (devices.IsNullOrEmpty())
                {
                    return;
                }

                var listDevices = devices.Split(',').ToList();

                if (!listDevices.Contains(devicesId)) return;
                this.Container.Resolve<ILogService>().Log($"Enable Reporter");
                this.Container.InstantiatePrefabResource("Reporter");
            });
#endif
        }
    }
}