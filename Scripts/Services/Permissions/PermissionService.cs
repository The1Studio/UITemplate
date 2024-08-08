namespace TheOneStudio.UITemplate.UITemplate.Services.Permissions
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Services.Permissions.Signals;
    using UnityEngine;
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

    // public interface IPermissionService
    // {
    //     // Only work for ANDROID devices
    //     UniTask<bool> RequestPermission(PermissionRequest permissionRequest);
    //     UniTask<bool> RequestPermission(UserAuthorization userAuthorization);
    // }
    public class PermissionService /*: IPermissionService*/
    {
        #region Inject

        private readonly ILogService logService;
        private readonly SignalBus signalBus;

        public PermissionService(ILogService logService, SignalBus signalBus)
        {
            this.logService = logService;
            this.signalBus = signalBus;
        }

        #endregion

        public async UniTask<bool> RequestPermission(PermissionRequest permissionRequest)
        {
            return await RequestPermissionInternal(permissionRequest, permissionRequest.ToPermissionString());
        }

        public async UniTask<bool> RequestPermission(UserAuthorization userAuthorization)
        {
            return await RequestPermissionInternal(userAuthorization, userAuthorization.ToString());
        }

        private async UniTask<bool> RequestPermissionInternal(object request, string permissionString)
        {
            this.signalBus.Fire<OnRequestPermissionStartSignal>();
            this.logService.Log($"onelog: CheckPermission Start: {request}");
            bool isGranted;

#if THEONE_NOTIFICATION
            if (request is PermissionRequest and PermissionRequest.Notification)
            {
                isGranted = await RequestNotificationPermission();
            }
            else
#endif
            {
#if UNITY_ANDROID
                isGranted = await RequestAndroidPermission(permissionString);
#else
                isGranted = await RequestUserAuthorization(permissionString);
#endif
            }

            this.signalBus.Fire(new OnRequestPermissionCompleteSignal() { IsGranted = isGranted });
            this.logService.Log($"onelog: CheckPermission Complete: {request} - isGranted: {isGranted}");
            return isGranted;
        }

        private static async UniTask<bool> RequestUserAuthorization(string userAuthorization)
        {
            if (System.Enum.TryParse(userAuthorization, out UserAuthorization authorization) && !Application.HasUserAuthorization(authorization))
            {
                await Application.RequestUserAuthorization(authorization);
            }
            return Application.HasUserAuthorization(authorization);
        }

#if THEONE_NOTIFICATION && UNITY_ANDROID
        private static async UniTask<bool> RequestNotificationPermission()
        {
            if (PermissionHelper.GetSDKVersionInt() < 28)
            {
                return AndroidNotificationCenter.UserPermissionToPost is not (PermissionStatus.RequestPending or PermissionStatus.NotRequested);
            }
            return false;
        }
#elif THEONE_NOTIFICATION && UNITY_IOS
        private static async UniTask<bool> RequestNotificationPermission()
        {
            var iOSNotificationSettings = iOSNotificationCenter.GetNotificationSettings();
            if (iOSNotificationSettings.AuthorizationStatus == AuthorizationStatus.NotDetermined)
            {
                using var req = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge, true);
                await UniTask.WaitUntil(() => req.IsFinished);
            }
            return iOSNotificationSettings.AuthorizationStatus != AuthorizationStatus.Denied;
        }
#endif

#if UNITY_ANDROID
        private static async UniTask<bool> RequestAndroidPermission(string permissionString)
        {
            if (!Permission.HasUserAuthorizedPermission(permissionString))
            {
                var isGranted = false;
                var permissionCallbacks = new PermissionCallbacks();

                permissionCallbacks.PermissionGranted += _ => isGranted = true;
                permissionCallbacks.PermissionDenied += _ => isGranted = false;
                permissionCallbacks.PermissionDeniedAndDontAskAgain += _ => isGranted = false;

                Permission.RequestUserPermission(permissionString, permissionCallbacks);

                await UniTask.WaitUntil(() => isGranted || Permission.HasUserAuthorizedPermission(permissionString));
                return isGranted || Permission.HasUserAuthorizedPermission(permissionString);
            }
            return true;
        }
#endif
    }
}
