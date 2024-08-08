namespace TheOneStudio.UITemplate.UITemplate.Services.Permissions
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.Utilities.LogService;
    using Unity.Notifications.Android;
    using UnityEngine.Android;
    using Zenject;

    public class AndroidPermissionService : BaseUnityPermissionService
    {
        public AndroidPermissionService(ILogService logService, SignalBus signalBus) : base(logService, signalBus) { }

        protected override async UniTask<bool> InternalRequestPermission(object request)
        {
            if (request is not PermissionRequest permissionRequest) return false;
            var permissionString  = permissionRequest.ToPermissionString();
            if (Permission.HasUserAuthorizedPermission(permissionString)) return true;
            var isGranted           = false;
            var permissionCallbacks = new PermissionCallbacks();

            permissionCallbacks.PermissionGranted               += _ => isGranted = true;
            permissionCallbacks.PermissionDenied                += _ => isGranted = false;
            permissionCallbacks.PermissionDeniedAndDontAskAgain += _ => isGranted = false;

            Permission.RequestUserPermission(permissionString, permissionCallbacks);

            await UniTask.WaitUntil(() => isGranted || Permission.HasUserAuthorizedPermission(permissionString));
            return isGranted || Permission.HasUserAuthorizedPermission(permissionString);
        }

        protected override async UniTask<bool> InternalRequestNotificationPermission()
        {
            if (PermissionHelper.GetSDKVersionInt() < 28)
            {
                return AndroidNotificationCenter.UserPermissionToPost is not (PermissionStatus.RequestPending or PermissionStatus.NotRequested);
            }

            return false;
        }
    }
}