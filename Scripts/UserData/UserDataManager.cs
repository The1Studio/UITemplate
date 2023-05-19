namespace TheOneStudio.UITemplate.UITemplate.UserData
{
    using System;
    using System.Linq;
    using System.Reflection;
    using GameFoundation.Scripts.Interfaces;
    using GameFoundation.Scripts.Utilities.Extension;
    using GameFoundation.Scripts.Utilities.UserData;
    using Sirenix.Utilities;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using Zenject;

    public class UserDataManager
    {
        private readonly DiContainer             container;
        private readonly SignalBus               signalBus;
        private readonly IHandleUserDataServices handleUserDataService;

        public UserDataManager(DiContainer container, SignalBus signalBus, IHandleUserDataServices handleUserDataService)
        {
            this.container             = container;
            this.signalBus             = signalBus;
            this.handleUserDataService = handleUserDataService;
        }

        public async void LoadUserData()
        {
            var types = ReflectionUtils.GetAllDerivedTypes<ILocalData>().ToArray();
            var datas = await this.handleUserDataService.Load(types);
            Enumerable.Zip(types, datas, (type, data) => (type, data)).ForEach(p =>
            {
                var type = p.type;
                var data = p.data;
                if (type.GetProperty(nameof(IUITemplateLocalData.ControllerType))?.GetValue(data) is Type controllerType)
                {
                    data.CopyTo(controllerType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                                              .First(fieldInfo => fieldInfo.FieldType == type)
                                              .GetValue(this.container.Resolve(controllerType))
                    );
                }
                else
                {
                    data.CopyTo(this.container.Resolve(type));
                }
            });
            this.signalBus.Fire<UserDataLoadedSignal>();
        }
    }
}