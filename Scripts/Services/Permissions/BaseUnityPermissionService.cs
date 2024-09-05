namespace TheOneStudio.UITemplate.UITemplate.Services.Permissions
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Services.Permissions.Signals;

    public abstract class BaseUnityPermissionService : IPermissionService
    {
        protected readonly ILogService LOGService;
        protected readonly SignalBus   SignalBus;

        protected BaseUnityPermissionService(ILogService logService, SignalBus signalBus)
        {
            this.LOGService = logService;
            this.SignalBus  = signalBus;
        }

        public virtual async UniTask<bool> RequestPermission(PermissionRequest request)
        {
            this.SignalBus.Fire<OnRequestPermissionStartSignal>();
            this.LOGService.Log($"oneLog: CheckPermission Start: {request}");
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
            this.LOGService.Log($"onelog: CheckPermission Complete: {request} - isGranted: {isGranted}");
            return isGranted;
        }

        protected abstract UniTask<bool> InternalRequestPermission(PermissionRequest request);

        protected abstract UniTask<bool> InternalRequestNotificationPermission();
    }
}