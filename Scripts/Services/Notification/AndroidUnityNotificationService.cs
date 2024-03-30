#if THEONE_NOTIFICATION && UNITY_ANDROID
namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using Core.AnalyticServices;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using Zenject;
    using UnityEngine;
    using System;
    using Unity.Notifications.Android;
    using UnityEngine.Android;
    using Cysharp.Threading.Tasks;

    public class AndroidUnityNotificationService : BaseUnityNotificationService
    {
        public AndroidUnityNotificationService(SignalBus signalBus, UITemplateNotificationBlueprint uiTemplateNotificationBlueprint,
            UITemplateNotificationDataBlueprint uiTemplateNotificationDataBlueprint, NotificationMappingHelper notificationMappingHelper, ILogService logger, IAnalyticServices analyticServices) :
            base(signalBus, uiTemplateNotificationBlueprint, uiTemplateNotificationDataBlueprint, notificationMappingHelper, logger, analyticServices)
        {
        }

        protected override void RegisterNotification()
        {
            var channel = new AndroidNotificationChannel(ChannelId, ChannelName, ChannelDescription, Importance.Default);
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
        }

        protected override async void CheckOpenedByNotification()
        {
            await UniTask.DelayFrame(1); // await 1 frame are required to get the last notification intent
            var intent = AndroidNotificationCenter.GetLastNotificationIntent();
            if (intent != null)
                this.TrackEventClick(new NotificationContent(intent.Notification.Title, intent.Notification.Text));
        }

        public override async UniTask CheckPermission()
        {
            var isWaitingForPermission = false;
            if (GetSDKVersionInt() >= 28 && !Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            {
                isWaitingForPermission = true;
                var permissionCallbacks = new PermissionCallbacks();
                permissionCallbacks.PermissionDenied                += _ => { isWaitingForPermission = false; };
                permissionCallbacks.PermissionDeniedAndDontAskAgain += _ => { isWaitingForPermission = false; };
                permissionCallbacks.PermissionGranted               += _ => { isWaitingForPermission = false; };
                Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS", permissionCallbacks);
                this.Logger.Log($"onelog: Notification RequestPermission: ");
            }

            await UniTask.WaitUntil(() => !isWaitingForPermission);

            var isPermissionAllow = AndroidNotificationCenter.UserPermissionToPost is not (PermissionStatus.RequestPending or PermissionStatus.NotRequested);

            this.Logger.Log($"onelog: Notification CheckPermission: {isPermissionAllow}");
        }

        public static int GetSDKVersionInt()
        {
#if !UNITY_EDITOR
            using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                return version.GetStatic<int>("SDK_INT");
            }
#else
            return 30;
#endif
        }

        public override void CancelNotification() { AndroidNotificationCenter.CancelAllNotifications(); }

        public override void SendNotification(string title, string body, DateTime fireTime, TimeSpan delayTime)
        {
            this.Logger.Log($"onelog: Notification SendNotification: {title} - {body} - {fireTime} - {delayTime}");
            var notification = new AndroidNotification
            {
                Title     = title,
                Text      = body,
                SmallIcon = SmallIcon,
                LargeIcon = LargeIcon,
                FireTime  = fireTime
            };
            AndroidNotificationCenter.SendNotification(notification, ChannelId);
        }
    }
}

#endif