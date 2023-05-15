namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using System;
    using GameFoundation.Scripts.Utilities.Extension;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Scripts.Utilities.UserData;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using Zenject;

    public class UITemplateLocalDataInstaller : Installer<UITemplateLocalDataInstaller>
    {
        public override void InstallBindings()
        {
            this.BindLocalData();
            //Data controller
            this.BindAllController();
        }

        private void BindLocalData()
        {
            var logger             = this.Container.Resolve<ILogService>();
            var handleDataServices = this.Container.Resolve<IHandleUserDataServices>();
            var listLocalData      = ReflectionUtils.GetAllDerivedTypes<IUITemplateLocalData>();

            foreach (var localDataType in listLocalData)
            {
                var localData = handleDataServices.Load(localDataType);
                var property  = localDataType.GetProperty("ControllerType");

                if (property.GetValue(localData) == null)
                {
                    logger.Error($"Waring, the local data {localDataType.Name} has no controller, consider to create new controller");
                    this.Container.Bind(localDataType).FromInstance(localData).AsCached();
                }
                else
                {
                    this.Container.Bind(localDataType).FromInstance(localData).WhenInjectedInto((Type)property.GetValue(localData));
                }
            }
        }

        private void BindAllController()
        {
            var listController = ReflectionUtils.GetAllDerivedTypes<IUITemplateControllerData>();

            foreach (var controller in listController)
            {
                this.Container.BindInterfacesAndSelfTo(controller).AsCached();
            }
        }
    }
}