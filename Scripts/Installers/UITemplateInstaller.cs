namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using ServiceImplementation.IAPServices;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.CollectionNew;
    using TheOneStudio.UITemplate.UITemplate.Services;
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

            UITemplateDeclareSignalInstaller.Install(this.Container);
            UITemplateServicesInstaller.Install(this.Container, this.toastCanvas);
            IapInstaller.Install(this.Container);
            UITemplateLocalDataInstaller.Install(this.Container); // bind after FBInstantInstaller for remote user data
            UITemplateThirdPartyInstaller.Install(this.Container); // bind after UITemplateLocalDataInstaller for local data analytics
            UITemplateAdsInstaller.Install(this.Container); // this depend on third party service signals
            NotificationInstaller.Install(this.Container);
            StoreRatingServiceInstaller.Install(this.Container);
            this.Container.BindInterfacesAndSelfTo<UITemplateIapServices>().AsCached().NonLazy();
        }
    }
}