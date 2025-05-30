namespace TheOneStudio.UITemplate.UITemplate.Services.Permissions
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Signals;
    using TheOne.Logging;
    using TheOneStudio.UITemplate.UITemplate.Services.Permissions.Signals;

    public abstract class BaseUnityPermissionService : IPermissionService
    {
        protected readonly ILogger   LogService;
        protected readonly SignalBus SignalBus;

        protected BaseUnityPermissionService(ILoggerManager loggerManager, SignalBus signalBus)
        {
            this.LogService = loggerManager.GetLogger(this);
            this.SignalBus  = signalBus;
        }

        public virtual async UniTask<bool> RequestPermission(PermissionRequest request)
        {
            this.SignalBus.Fire<OnRequestPermissionStartSignal>();
            this.LogService.Info($"CheckPermission Start: {request}");
            bool isGranted;
            if (request is PermissionRequest.Notification)
            {
                #if THEONE_NOTIFICATION
                isGranted = await this.InternalRequestNotificationPermission();
                #else
                this.LOGService.Log($"oneLog: You must add THEONE_NOTIFICATION symbol to request notification permission!");
                isGranted = false;
                #endif
            }
            else
            {
                isGranted = await this.InternalRequestPermission(request);
            }

            this.SignalBus.Fire(new OnRequestPermissionCompleteSignal { IsGranted = isGranted });
            this.LogService.Info($"CheckPermission Complete: {request} - isGranted: {isGranted}");
            return isGranted;
        }

        protected abstract UniTask<bool> InternalRequestPermission(PermissionRequest request);

        protected abstract UniTask<bool> InternalRequestNotificationPermission();
    }
}