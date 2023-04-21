namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.Utilities.LogService;
    using ServiceImplementation.IAPServices;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TheOneStudio.UITemplate.UITemplate.FTUE;
    using TheOneStudio.UITemplate.UITemplate.Interfaces;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.Decoration;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Signals;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using UnityEngine;
    using Zenject;

    public class UITemplateInstaller : Installer<GameObject, UITemplateInstaller>
    {
        private readonly GameObject soundGroupPrefab;

        public UITemplateInstaller(GameObject soundGroupPrefab) { this.soundGroupPrefab = soundGroupPrefab; }

        public override void InstallBindings()
        {
            //Helper
            this.Container.Bind<UITemplateAnimationHelper>().AsCached();

            UITemplateDeclareSignalInstaller.Install(this.Container);
            UITemplateLocalDataInstaller.Install(this.Container);
            UnityIapInstaller.Install(this.Container);
            FTUEInstaller.Install(this.Container);
            UITemplateServicesInstaller.Install(this.Container, this.soundGroupPrefab);
            UITemplateThirdPartyInstaller.Install(this.Container);
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
            var debugKey    = "DebugDevices";

            this.Container.Resolve<SignalBus>().Subscribe<RemoteConfigInitializeSucceededSignal>(() =>
            {
                var remoteConfig = this.Container.Resolve<IUITemplateRemoteConfig>();
                var devices      = remoteConfig.GetRemoteConfigStringValue(debugKey,"");

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