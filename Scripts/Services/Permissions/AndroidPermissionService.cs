namespace TheOneStudio.UITemplate.UITemplate.Services.Permissions
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Signals;
    using TheOne.Logging;
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
        public AndroidPermissionService(ILoggerManager loggerManager, SignalBus signalBus) : base(loggerManager, signalBus)
        {
        }

        #pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        protected override async UniTask<bool> InternalRequestPermission(PermissionRequest request)
        {
            #if UNITY_ANDROID
            var permissionString = request.ToPermissionString();
            if (Permission.HasUserAuthorizedPermission(permissionString)) return true;
            var isGranted           = false;
            var permissionCallbacks = new PermissionCallbacks();

            permissionCallbacks.PermissionGranted               += _ => isGranted = true;
            permissionCallbacks.PermissionDenied                += _ => isGranted = false;
            #pragma warning disable CS0618 // Type or member is obsolete
            permissionCallbacks.PermissionDeniedAndDontAskAgain += _ => isGranted = false;
            #pragma warning restore CS0618 // Type or member is obsolete

            Permission.RequestUserPermission(permissionString, permissionCallbacks);

            await UniTask.WaitUntil(() => isGranted || Permission.HasUserAuthorizedPermission(permissionString));
            return isGranted || Permission.HasUserAuthorizedPermission(permissionString);
            #else
            return false;
            #endif
        }

        protected override async UniTask<bool> InternalRequestNotificationPermission()
        {
            #if THEONE_NOTIFICATION && UNITY_ANDROID
            if (PermissionHelper.GetSDKVersionInt() < 28) return AndroidNotificationCenter.UserPermissionToPost is not (PermissionStatus.RequestPending or PermissionStatus.NotRequested);
            #endif
            this.LogService.Info($"You must add THEONE_NOTIFICATION symbol to request notification permission!");
            return false;
        }
        #pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}