﻿namespace TheOneStudio.UITemplate.UITemplate.Services.Permissions
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Signals;
    using TheOne.Logging;
    using TheOneStudio.UITemplate.UITemplate.Services.Permissions.Signals;

    public abstract class BaseUnityPermissionService : IPermissionService
    {
        protected readonly ILogger   Logger;
        protected readonly SignalBus SignalBus;

        protected BaseUnityPermissionService(ILoggerManager loggerManager, SignalBus signalBus)
        {
            this.Logger    = loggerManager.GetLogger(this);
            this.SignalBus = signalBus;
        }

        public virtual async UniTask<bool> RequestPermission(PermissionRequest request)
        {
            this.SignalBus.Fire<OnRequestPermissionStartSignal>();
            this.Logger.Info($"CheckPermission Start: {request}");
            bool isGranted;
            if (request is PermissionRequest.Notification)
            {
                #if THEONE_NOTIFICATION
                isGranted = await this.InternalRequestNotificationPermission();
                #else
                this.Logger.Log($"You must add THEONE_NOTIFICATION symbol to request notification permission!", LogLevel.Warning);
                isGranted = false;
                #endif
            }
            else
            {
                isGranted = await this.InternalRequestPermission(request);
            }

            this.SignalBus.Fire(new OnRequestPermissionCompleteSignal { IsGranted = isGranted });
            this.Logger.Info($"CheckPermission Complete: {request} - isGranted: {isGranted}");
            return isGranted;
        }

        protected abstract UniTask<bool> InternalRequestPermission(PermissionRequest request);

        protected abstract UniTask<bool> InternalRequestNotificationPermission();
    }
}