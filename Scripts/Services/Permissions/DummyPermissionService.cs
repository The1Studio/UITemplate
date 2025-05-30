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

        protected override UniTask<bool> InternalRequestPermission(PermissionRequest request)
        {
            this.LogService.Info("DummyPermissionService InternalRequestPermission: {request}");
            return UniTask.FromResult(false);
        }

        protected override UniTask<bool> InternalRequestNotificationPermission()
        {
            this.LogService.Info("DummyPermissionService InternalRequestNotificationPermission");
            return UniTask.FromResult(false);
        }
    }
}