namespace TheOneStudio.UITemplate.UITemplate.Scenes.BadgeNotify
{
    using GameFoundation.Scripts.Utilities.Extension;
    using Zenject;

    public class UITemplateBadgeNotifyInstaller : Installer<UITemplateBadgeNotifyInstaller>
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfToAllTypeDriveFrom<IBadgeScreenRegister>();
        }
    }
}