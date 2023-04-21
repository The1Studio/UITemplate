namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using Zenject;

    public class UITemplateLocalDataInstaller : Installer<UITemplateLocalDataInstaller>
    {
        public override void InstallBindings()
        {
            this.Container.BindLocalData<UITemplateUserLevelData>();
            this.Container.BindLocalData<UITemplateInventoryData>();
            this.Container.BindLocalData<UITemplateUserSettingData>();
            this.Container.BindLocalData<UITemplateDailyRewardData>();
            this.Container.BindLocalData<UITemplateUserJackpotData>();
            this.Container.BindLocalData<UITemplateAdsData>();
            this.Container.BindLocalData<UITemplateLuckySpinData>();
            this.Container.BindLocalData<UITemplateBuildingData>();
            this.Container.BindLocalData<UITemplateRewardData>();
            this.Container.BindLocalData<UITemplateCommonData>();
            this.Container.BindLocalData<UITemplateIAPOwnerPackData>();
            this.Container.BindLocalData<UITemplateFTUEData>();

            //Data controller
            this.BindAllController();
        }

        private void BindAllController()
        {
            var listController = ReflectionUtils.GetAllDerivedTypes<IUITemplateControllerData>();

            foreach (var localDataType in listController)
            {
                this.Container.BindInterfacesAndSelfTo(localDataType).AsCached();
            }
        }
    }
}