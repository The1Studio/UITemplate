namespace TheOneStudio.UITemplate.UITemplate.Services.Permissions
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Services.Permissions.Signals;
    using Zenject;

#if UNITY_ANDROID
    using UnityEngine.Android;
#endif
#if THEONE_NOTIFICATION && UNITY_ANDROID
    using Unity.Notifications.Android;
#endif

#if THEONE_NOTIFICATION && UNITY_IOS
    using Unity.Notifications.iOS;
#endif

    public class PermissionService
    {
        #region Inject

        private readonly ILogService logService;
        private readonly SignalBus   signalBus;

        public PermissionService(ILogService logService, SignalBus signalBus)
        {
            this.logService = logService;
            this.signalBus  = signalBus;
        }

        #endregion

        public async UniTask<bool> RequestPermission(PermissionRequest permissionRequest)
        {
            this.signalBus.Fire<OnRequestPermissionStartSignal>();
            this.logService.Log($"onelog: CheckPermission Start: {permissionRequest}");
            var isGranted = await InternalRequestPermission(permissionRequest);
            this.signalBus.Fire(new OnRequestPermissionCompleteSignal() { IsGranted = isGranted });
            this.logService.Log($"onelog: CheckPermission Complete: {permissionRequest} - isGranted: {isGranted}");
            return isGranted;
        }

        private static async UniTask<bool> InternalRequestPermission(PermissionRequest permissionRequest)
        {
            var isWaitingForPermission = false;
            var isGranted              = false;
            var permissionString       = permissionRequest.ToPermissionString();

            #region Notification

            if (permissionRequest == PermissionRequest.Notification)
            {
#if THEONE_NOTIFICATION && UNITY_ANDROID
                if (PermissionHelper.GetSDKVersionInt() < 28)
                {
                    return AndroidNotificationCenter.UserPermissionToPost is not (PermissionStatus.RequestPending or PermissionStatus.NotRequested);
                }
#elif THEONE_NOTIFICATION && UNITY_IOS
                var iOSNotificationSettings = iOSNotificationCenter.GetNotificationSettings();
                if (iOSNotificationSettings.AuthorizationStatus == AuthorizationStatus.NotDetermined)
                {
                    using var req = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge, true);
                    await UniTask.WaitUntil(() => req.IsFinished);
                }

                return iOSNotificationSettings.AuthorizationStatus != AuthorizationStatus.Denied;
#endif
            }

            #endregion

#if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(permissionString))
            {
                isWaitingForPermission = true;
                var permissionCallbacks = new PermissionCallbacks();
                permissionCallbacks.PermissionDenied                += _ => { isWaitingForPermission = false; };
                permissionCallbacks.PermissionDeniedAndDontAskAgain += _ => { isWaitingForPermission = false; };
                permissionCallbacks.PermissionGranted += _ =>
                {
                    isWaitingForPermission = false;
                    isGranted              = true;
                };
                Permission.RequestUserPermission(permissionString, permissionCallbacks);
            }

            await UniTask.WaitUntil(() => !isWaitingForPermission);
#endif

            return isGranted;
        }
    }
}