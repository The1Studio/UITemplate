namespace TheOneStudio.UITemplate.UITemplate.Services.Permissions
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Signals;
    using TheOne.Logging;
    using UnityEngine.Scripting;

    public class DummyPermissionService : BaseUnityPermissionService
    {
        [Preserve]
        public DummyPermissionService(ILoggerManager loggerManager, SignalBus signalBus) : base(loggerManager, signalBus)
        {
        }

        protected override async UniTask<bool> InternalRequestPermission(PermissionRequest request)
        {
            this.LogService.Info("DummyPermissionService InternalRequestPermission: {request}");
            return false;
        }

        protected override async UniTask<bool> InternalRequestNotificationPermission()
        {
            this.LogService.Info("DummyPermissionService InternalRequestNotificationPermission");
            return false;
        }
    }
}