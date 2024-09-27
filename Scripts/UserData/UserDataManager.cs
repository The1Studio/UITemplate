namespace TheOneStudio.UITemplate.UITemplate.UserData
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Cysharp.Threading.Tasks;
    using GameFoundation.DI;
    using GameFoundation.Scripts.Interfaces;
    using GameFoundation.Scripts.Utilities.Extension;
    using GameFoundation.Scripts.Utilities.UserData;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using UnityEngine.Scripting;

    public class UserDataManager
    {
        private readonly IDependencyContainer    container;
        private readonly SignalBus               signalBus;
        private readonly IHandleUserDataServices handleUserDataService;

        [Preserve]
        public UserDataManager(IDependencyContainer container, SignalBus signalBus, IHandleUserDataServices handleUserDataService)
        {
            this.container             = container;
            this.signalBus             = signalBus;
            this.handleUserDataService = handleUserDataService;
        }

        public async UniTask LoadUserData()
        {
            var types     = ReflectionUtils.GetAllDerivedTypes<ILocalData>().ToArray();
            var datas     = await this.handleUserDataService.Load(types);
            var dataCache = (Dictionary<string, ILocalData>)typeof(BaseHandleUserDataServices).GetField("userDataCache", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(this.handleUserDataService);
            IterTools.Zip(types, datas).ForEach((type, data) =>
            {
                var boundData = (data as IUITemplateLocalData)?.ControllerType is { } controllerType
                    ? controllerType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                        .First(fieldInfo => fieldInfo.FieldType == type)
                        .GetValue(this.container.Resolve(controllerType))
                    : this.container.Resolve(type);

                data.CopyTo(boundData);
                dataCache[BaseHandleUserDataServices.KeyOf(type)] = (ILocalData)boundData;
            });
            this.signalBus.Fire<UserDataLoadedSignal>();
        }
    }
}