namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using System;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.Interfaces;
    using GameFoundation.Scripts.Utilities.Extension;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Scripts.Utilities.UserData;
    using ModestTree;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using Zenject;

    public class UITemplateLocalDataInstaller : Installer<UITemplateLocalDataInstaller>
    {
        public override void InstallBindings()
        {
            this.BindLocalData().ContinueWith(this.BindAllController).Forget();
        }

        private async UniTask BindLocalData()
        {
            var logger            = this.Container.Resolve<ILogService>();
            var handleDataService = this.Container.Resolve<IHandleUserDataServices>();
            var listLocalData     = ReflectionUtils.GetAllDerivedTypes<ILocalData>();

            foreach (var localDataType in listLocalData)
            {
                var localData = await handleDataService.Load(localDataType);

                if (localDataType.DerivesFrom<IUITemplateLocalData>())
                {
                    if (localDataType.GetProperty(nameof(IUITemplateLocalData.ControllerType))?.GetValue(localData) is Type controllerType)
                    {
                        this.Container.Bind(localDataType).FromInstance(localData).WhenInjectedInto(controllerType);
                    }
                    else
                    {
                        logger.Error($"Waring, the local data {localDataType.Name} has no controller, consider to create new controller");
                        this.Container.Bind(localDataType).FromInstance(localData).AsCached();
                    }
                }
                else
                {
                    this.Container.Bind(localDataType).FromInstance(localData).AsCached();
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