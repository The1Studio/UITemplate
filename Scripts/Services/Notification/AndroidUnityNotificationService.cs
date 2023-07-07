namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using Core.AnalyticServices;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using Unity.Notifications.Android;
    using UnityEngine.Android;
    using Zenject;

    public class AndroidUnityNotificationService : BaseUnityNotificationService
    {
        public AndroidUnityNotificationService(SignalBus signalBus, UITemplateNotificationBlueprint uiTemplateNotificationBlueprint,
            UITemplateNotificationDataBlueprint uiTemplateNotificationDataBlueprint, ILogService logger, IAnalyticServices analyticServices) : base(signalBus, uiTemplateNotificationBlueprint,
            uiTemplateNotificationDataBlueprint, logger, analyticServices)
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

        protected override async UniTask CheckPermission()
        {
            var isWaitingForPermission = false;
            if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
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

        protected override void CancelNotification() { AndroidNotificationCenter.CancelAllNotifications(); }

        protected override void SendNotification(string title, string body, DateTime fireTime, TimeSpan delayTime)
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