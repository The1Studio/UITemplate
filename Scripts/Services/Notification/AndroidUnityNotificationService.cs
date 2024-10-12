#if THEONE_NOTIFICATION && UNITY_ANDROID
namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using Core.AnalyticServices;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using System;
    using Unity.Notifications.Android;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Services.Permissions;
    using UnityEngine.Scripting;

    public class AndroidUnityNotificationService : BaseUnityNotificationService
    {
        [Preserve]
        public AndroidUnityNotificationService(
            SignalBus                           signalBus,
            UITemplateNotificationBlueprint     uiTemplateNotificationBlueprint,
            UITemplateNotificationDataBlueprint uiTemplateNotificationDataBlueprint,
            NotificationMappingHelper           notificationMappingHelper,
            ILogService                         logger,
            IAnalyticServices                   analyticServices,
            IPermissionService                  permissionService
        ) :
            base(signalBus, uiTemplateNotificationBlueprint, uiTemplateNotificationDataBlueprint, notificationMappingHelper, logger, analyticServices, permissionService)
        {
        }

        protected override void RegisterNotification()
        {
            var channel = new AndroidNotificationChannel(this.ChannelId, this.ChannelName, this.ChannelDescription, Importance.Default);
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
        }

        protected override async void CheckOpenedByNotification()
        {
            await UniTask.DelayFrame(1); // await 1 frame are required to get the last notification intent
            var intent = AndroidNotificationCenter.GetLastNotificationIntent();
            if (intent != null) this.TrackEventClick(new(intent.Notification.Title, intent.Notification.Text));
        }

        public override void CancelNotification()
        {
            AndroidNotificationCenter.CancelAllNotifications();
        }

        public override void SendNotification(string title, string body, DateTime fireTime, TimeSpan delayTime)
        {
            this.Logger.Log($"onelog: Notification SendNotification: {title} - {body} - {fireTime} - {delayTime}");
            var notification = new AndroidNotification
            {
                Title     = title,
                Text      = body,
                SmallIcon = this.SmallIcon,
                LargeIcon = this.LargeIcon,
                FireTime  = fireTime,
            };
            AndroidNotificationCenter.SendNotification(notification, this.ChannelId);
        }
    }
}

#endif