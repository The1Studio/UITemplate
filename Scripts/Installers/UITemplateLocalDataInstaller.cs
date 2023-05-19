namespace TheOneStudio.UITemplate.UITemplate.Installers
{
    using System;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.Interfaces;
    using GameFoundation.Scripts.Utilities.Extension;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Scripts.Utilities.UserData;
    using ModestTree;
    using Sirenix.Utilities;
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
            var types             = ReflectionUtils.GetAllDerivedTypes<ILocalData>().ToArray();
            var datas             = await handleDataService.Load(types);

            Enumerable.Zip(types, datas, (type, data) => (type, data)).ForEach(p =>
            {
                var type = p.type;
                var data = p.data;
                if (type.DerivesFrom<IUITemplateLocalData>())
                {
                    if (type.GetProperty(nameof(IUITemplateLocalData.ControllerType))?.GetValue(data) is Type controllerType)
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