namespace TheOneStudio.UITemplate.UITemplate.Installers
{
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
            //Data controller
            this.Container.BindInterfacesAndSelfTo<UITemplateDailyRewardController>().AsCached();
            this.Container.BindInterfacesAndSelfTo<UITemplateInventoryDataController>().AsCached();
            this.Container.BindInterfacesAndSelfTo<UITemplateLevelDataController>().AsCached();
            this.Container.BindInterfacesAndSelfTo<UITemplateSettingDataController>().AsCached();
            this.Container.BindInterfacesAndSelfTo<UITemplateJackpotController>().AsCached();
            this.Container.BindInterfacesAndSelfTo<UITemplateLuckySpinController>().AsCached();
            this.Container.BindInterfacesAndSelfTo<UITemplateBuildingController>().AsCached();
            this.Container.BindInterfacesAndSelfTo<UITemplateHandleRewardController>().AsCached();
        }
    }
}