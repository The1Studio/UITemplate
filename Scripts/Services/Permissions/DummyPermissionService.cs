﻿namespace TheOneStudio.UITemplate.UITemplate.Services.Permissions
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.Utilities.LogService;
    using Zenject;

    public class DummyPermissionService : BaseUnityPermissionService
    {
        public DummyPermissionService(ILogService logService, SignalBus signalBus) : base(logService, signalBus)
        {
        }

        protected override async UniTask<bool> InternalRequestPermission(PermissionRequest request)
        {
            this.LOGService.Log($"oneLog: DummyPermissionService InternalRequestPermission: {request}");
            return false;
        }

        protected override async UniTask<bool> InternalRequestNotificationPermission()
        {
            this.LOGService.Log($"oneLog: DummyPermissionService InternalRequestNotificationPermission");
            return false;
        }
    }
}