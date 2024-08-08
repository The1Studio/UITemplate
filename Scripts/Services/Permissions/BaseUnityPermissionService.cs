namespace TheOneStudio.UITemplate.UITemplate.Services.Permissions
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Services.Permissions.Signals;
    using Zenject;

    public abstract class BaseUnityPermissionService : IPermissionService
    {
        private readonly ILogService logService;
        private readonly SignalBus   signalBus;

        protected BaseUnityPermissionService(ILogService logService, SignalBus signalBus)
        {
            this.logService = logService;
            this.signalBus  = signalBus;
        }

        public virtual async UniTask<bool> RequestPermission(object request)
        {
            this.signalBus.Fire<OnRequestPermissionStartSignal>();
            this.logService.Log($"oneLog: CheckPermission Start: {request}");
            bool isGranted;
            if (request is PermissionRequest.Notification)
            {
#if THEONE_NOTIFICATION
                isGranted = await this.InternalRequestNotificationPermission();
#else
                isGranted = false;
                this.logService.Log($"oneLog: You must add THEONE_NOTIFICATION symbol to request notification permission!");
#endif 
            }
            else
            {
                isGranted = await this.InternalRequestPermission(request);
            }

            this.signalBus.Fire(new OnRequestPermissionCompleteSignal { IsGranted = isGranted });
            this.logService.Log($"onelog: CheckPermission Complete: {request} - isGranted: {isGranted}");
            return isGranted;
        }

        protected abstract UniTask<bool> InternalRequestPermission(object request);

        protected abstract UniTask<bool> InternalRequestNotificationPermission();
    }
}