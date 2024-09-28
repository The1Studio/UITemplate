#if GDK_ZENJECT
namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using System;
    using GameFoundation.Scripts.Interfaces;
    using GameFoundation.Scripts.Utilities.Extension;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using TheOneStudio.UITemplate.UITemplate.UserData;
    using Zenject;
    using Zenject.Internal;

    public class UITemplateLocalDataInstaller : Installer<UITemplateLocalDataInstaller>
    {
        public override void InstallBindings()
        {
            this.BindLocalData();
            this.BindAllController();
        }

        private void BindLocalData()
        {
            var logger = this.Container.Resolve<ILogService>();

            ReflectionUtils.GetAllDerivedTypes<ILocalData>().ForEach(type =>
            {
                var data = Activator.CreateInstance(type);
                if (type.DerivesFrom<IUITemplateLocalData>())
                {
                    if ((data as IUITemplateLocalData)?.ControllerType is { } controllerType)
                    {
                        this.Container.Bind(type).FromInstance(data).WhenInjectedInto(controllerType);
                    }
                    else
                    {
                        logger.Error($"Waring, the local data {type.Name} has no controller, consider to create new controller");
                        this.Container.Bind(type).FromInstance(data).AsCached();
                    }
                }
                else
                {
                    this.Container.Bind(type).FromInstance(data).AsCached();
                }
            });

            this.Container.Bind<UserDataManager>().AsCached();
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
#endif