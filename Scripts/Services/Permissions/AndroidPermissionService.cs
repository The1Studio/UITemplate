namespace TheOneStudio.UITemplate.UITemplate.Services.Permissions
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Signals;
    using UnityEngine.Scripting;
    #if UNITY_ANDROID
    using UnityEngine.Android;
#endif
#if THEONE_NOTIFICATION && UNITY_ANDROID
    using Unity.Notifications.Android;
#endif

    public class AndroidPermissionService : BaseUnityPermissionService
    {
        [Preserve]
        public AndroidPermissionService(ILogService logService, SignalBus signalBus) : base(logService, signalBus) { }

        protected override async UniTask<bool> InternalRequestPermission(PermissionRequest request)
        {
#if UNITY_ANDROID
            var permissionString = request.ToPermissionString();
            if (Permission.HasUserAuthorizedPermission(permissionString)) return true;
            var isGranted = false;
            var permissionCallbacks = new PermissionCallbacks();

            permissionCallbacks.PermissionGranted += _ => isGranted = true;
            permissionCallbacks.PermissionDenied += _ => isGranted = false;
            permissionCallbacks.PermissionDeniedAndDontAskAgain += _ => isGranted = false;

            Permission.RequestUserPermission(permissionString, permissionCallbacks);

            await UniTask.WaitUntil(() => isGranted || Permission.HasUserAuthorizedPermission(permissionString));
            return isGranted || Permission.HasUserAuthorizedPermission(permissionString);
#endif
            return false;
        }

        protected override async UniTask<bool> InternalRequestNotificationPermission()
        {
#if THEONE_NOTIFICATION && UNITY_ANDROID
            if (PermissionHelper.GetSDKVersionInt() < 28)
            {
                return AndroidNotificationCenter.UserPermissionToPost is not (PermissionStatus.RequestPending or PermissionStatus.NotRequested);
            }
#endif
            this.LOGService.Log($"oneLog: You must add THEONE_NOTIFICATION symbol to request notification permission!");
            return false;
        }
    }
}